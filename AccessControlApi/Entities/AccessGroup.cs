namespace AccessControlApi.Entities;

public class AccessGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<Reader> Readers { get; set; } = new();
    public List<User> Users { get; set; } = new();

    public List<AccessRule> Rules { get; set; } = new();
}