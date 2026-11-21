namespace AccessControlApi.Entities;

public class AccessGroup
{
    //godziny dostepu
    //
    
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<Reader> Readers = new();
    public List<User> Users = new();
    
    public bool Monday { get; set; }
    public bool Tuesday { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday { get; set; }
    public bool Friday { get; set; }
    public bool Saturday { get; set; }
    public bool Sunday { get; set; }
    
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}