namespace Domain.Entities;

public class ProgressBook : BaseEntity
{
    public int Grade { get; set; }
    public bool IsAttended { get; set; }
    public string Note { get; set; } = null!;
    public int LateMinutes { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int GroupId { get; set; }
    public Group? Group { get; set; }
    public int TimeTableId { get; set; }
    public TimeTable? TimeTable { get; set; }
}