namespace AccessControlApi.Entities;

public class AccessRule
{
    public int Id { get; set; }
    
    public int AccessGroupId { get; set; }
    public AccessGroup AccessGroup { get; set; } = null!;
    
    public DayOfWeek Day  { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    
}