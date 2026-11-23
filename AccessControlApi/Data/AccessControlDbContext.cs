using Microsoft.EntityFrameworkCore;
using AccessControlApi.Entities;

namespace AccessControlApi.Data;

public class AccessControlDbContext : DbContext
{
    public AccessControlDbContext(
        DbContextOptions<AccessControlDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Reader> Readers => Set<Reader>();
    public DbSet<AccessGroup> AccessGroups => Set<AccessGroup>();
    public DbSet<AccessEvent> Events => Set<AccessEvent>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Card>(e =>
        {
            e.HasIndex(c => c.Uid).IsUnique();
            e.Property(c => c.Uid).HasMaxLength(64);

            e.HasOne(c => c.User)
                .WithMany(u => u.Cards)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);   // usunięcie użytkownika odpina karty
        });

        modelBuilder.Entity<User>(e =>
        {
            e.Property(u => u.Name).HasMaxLength(200);

            e.HasOne(u => u.AccessGroup)
                .WithMany(g => g.Users)
                .HasForeignKey(u => u.AccessGroupId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AccessGroup>(e =>
        {
            e.Property(g => g.Name).HasMaxLength(100);
            
            e.HasMany(g => g.Readers).WithMany();
        });

        modelBuilder.Entity<AccessEvent>(e =>
        {
            e.Property(x => x.ScannedCardUid).HasMaxLength(64);
            e.Property(x => x.AccessResult).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.DenyReason).HasConversion<string>().HasMaxLength(30);

            // historia nie może znikać razem z użytkownikiem, kartą, czytnikiem
            e.HasOne(x => x.Reader).WithMany().HasForeignKey(x => x.ReaderId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.Card).WithMany().HasForeignKey(x => x.CardId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasIndex(x => x.Timestamp);
            e.HasIndex(x => new { x.ReaderId, x.Timestamp });
            e.HasIndex(x => new { x.UserId, x.Timestamp });
        });

        modelBuilder.Entity<AccessRule>(e =>
        {
            e.HasOne(r => r.AccessGroup)
                .WithMany(g => g.Rules)
                .HasForeignKey(r => r.AccessGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}