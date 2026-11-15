using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Employees;

public class UpdateEmployeeDto
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = null!;

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = null!;

    [MaxLength(30)]
    public string? Phone { get; set; }

    public DateTime HireDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Salary { get; set; }

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; set; }

    [Range(1, int.MaxValue)]
    public int PositionId { get; set; }
}