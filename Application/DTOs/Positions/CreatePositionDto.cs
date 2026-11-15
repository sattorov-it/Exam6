using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Positions;

public class CreatePositionDto
{
    [Required, MaxLength(100)]
    public string Title { get; set; } = null!;

    [Range(0, double.MaxValue)]
    public decimal MinSalary { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MaxSalary { get; set; }
}