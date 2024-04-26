using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs;

public class AddTimeTableDto
{
    [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]|[0-9]):[0-5][0-9]$", ErrorMessage = "Use format HH:MM only")]
    public required string FromTime { get; set; }
    [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]|[0-9]):[0-5][0-9]$", ErrorMessage = "Use format HH:MM only")]
    public required string ToTime { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    
}