namespace Domain.Models;

public class Position
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
}