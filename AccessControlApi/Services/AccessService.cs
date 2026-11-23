using AccessControlApi.Data;
using AccessControlApi.Entities;
using AccessControlApi.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace AccessControlApi.Services;

public record AccessDecision(AccessResult Result, DenyReason? Reason);

public class AccessService
{
    private static readonly TimeZoneInfo Tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Warsaw");
    private readonly AccessControlDbContext _db;

    public AccessService(AccessControlDbContext db) => _db = db;

    public async Task<AccessDecision?> ProcessScanAsync(string location, string uid)
    {
        var reader = await _db.Readers.FirstOrDefaultAsync(r => r.Location == location);
        if (reader is null) return null;   // nieznany czytnik

        var card = await _db.Cards
            .Include(c => c.User).ThenInclude(u => u!.AccessGroup).ThenInclude(g => g!.Readers)
            .Include(c => c.User).ThenInclude(u => u!.AccessGroup).ThenInclude(g => g!.Rules)
            .FirstOrDefaultAsync(c => c.Uid == uid);

        var decision = Decide(card, reader.Id);

        _db.Events.Add(new AccessEvent
        {
            Timestamp = DateTime.UtcNow,
            ReaderId = reader.Id,
            CardId = card?.Id,
            UserId = card?.UserId,
            ScannedCardUid = uid,
            AccessResult = decision.Result,
            DenyReason = decision.Reason
        });
        await _db.SaveChangesAsync();

        return decision;
    }

    private static AccessDecision Decide(Card? card, int readerId)
    {
        static AccessDecision Deny(DenyReason r) => new(AccessResult.Denied, r);

        if (card is null) return Deny(DenyReason.UnknownCard);      // nieznana karta DENY
        if (!card.IsActive) return Deny(DenyReason.CardInactive);   // karta nieaktywna DENY

        var group = card.User?.AccessGroup;
        if (group is null) return Deny(DenyReason.NoAccessGroup);   // uzytkownik nie jest w zadnej grupie
        if (!group.Readers.Any(r => r.Id == readerId)) return Deny(DenyReason.Other);   // uzytkownik nie jest w grupie z dostępem

        var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Tz);
        if (!IsAccessAllowed(group, localNow)) return Deny(DenyReason.OutsideSchedule);

        return new AccessDecision(AccessResult.Granted, null);
    }

    private static bool IsAccessAllowed(AccessGroup g, DateTime localNow)
    {
        var time = TimeOnly.FromDateTime(localNow);
        var today = localNow.DayOfWeek;

        return g.Rules.Any(r =>
            r.Day == today &&
            time >= r.StartTime &&
            (r.EndTime == TimeOnly.MinValue || time < r.EndTime));   // 00:00 = do końca dnia
    }
    
}