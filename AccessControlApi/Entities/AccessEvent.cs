using AccessControlApi.Entities.Enums;

namespace AccessControlApi.Entities;

public class AccessEvent
{
    public long Id { get; set; }
    
    public DateTime Timestamp { get; set; }
    
    public int? ReaderId { get; set; }
    public Reader? Reader { get; set; }
    
    public int? CardId { get; set; }
    public Card? Card { get; set; }
    
    public int? UserId { get; set; }
    public User? User { get; set; }

    public string ScannedCardUid { get; set; } = "";
    
    public AccessResult AccessResult { get; set; }
    public DenyReason? DenyReason { get; set; }
    
}