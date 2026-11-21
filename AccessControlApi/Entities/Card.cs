namespace AccessControlApi.Entities;

public class Card
{
    public int Id { get; set; }
    public string Uid { get; set; } = ""; 
    public int? UserId { get; set; }
    public User? User { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}