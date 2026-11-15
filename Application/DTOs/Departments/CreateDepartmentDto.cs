using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Departments;

public class CreateDepartmentDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }
}