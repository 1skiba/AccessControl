namespace AccessControlApi.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    
    public int? AccessGroupId { get; set; }
    public AccessGroup? AccessGroup { get; set; }

    public List<Card> Cards { get; set; } = new();
}