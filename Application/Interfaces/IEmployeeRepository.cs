using Application.DTOs.Employees;
using Domain.Models;

namespace Application.Interfaces;

public interface IEmployeeRepository
{
    Task<List<GetEmployeeDto>> GetEmployeesAsync(int? departmentId, int? positionId, string? search, int page, int pageSize, CancellationToken ct);
    Task<int> GetEmployeesCountAsync(int? departmentId, int? positionId, string? search, CancellationToken ct);
    Task<GetEmployeeDto?> GetEmployeeDtoAsync(int id, CancellationToken ct);
    Task<Employee?> GetEmployeeAsync(int id, CancellationToken ct);
    Task<Employee?> GetEmployeeByEmailAsync(string email, CancellationToken ct);
    Task<int> AddEmployeeAsync(Employee employee, CancellationToken ct);
    Task UpdateEmployeeAsync(Employee employee, CancellationToken ct);
    Task TerminateEmployeeAsync(int id, CancellationToken ct);
}