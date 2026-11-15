using Domain.Models;

namespace Application.Interfaces;

public interface IDepartmentRepository
{
    Task<List<Department>> GetDepartmentsAsync(CancellationToken ct);
    Task<Department?> GetDepartmentAsync(int id, CancellationToken ct);
    Task<Department?> GetDepartmentByNameAsync(string name, CancellationToken ct);
    Task<int> AddDepartmentAsync(Department department, CancellationToken ct);
    Task UpdateDepartmentAsync(Department department, CancellationToken ct);
    Task DeleteDepartmentAsync(int id, CancellationToken ct);
    Task<bool> HasEmployeesAsync(int id, CancellationToken ct);
}