namespace Domain.Entities;

public class TimeTable:BaseEntity
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan  FromTime { get; set; }
    public TimeSpan  ToTime { get; set; }
    
}