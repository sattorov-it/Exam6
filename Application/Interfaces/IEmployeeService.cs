using Application.DTOs;
using Application.DTOs.Employees;

namespace Application.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<GetEmployeeDto>> GetEmployeesAsync(int? departmentId, int? positionId, string? search, int page, int pageSize, CancellationToken ct);
    Task<GetEmployeeDto> GetEmployeeAsync(int id, CancellationToken ct);
    Task<GetEmployeeDto> AddEmployeeAsync(CreateEmployeeDto dto, CancellationToken ct);
    Task UpdateEmployeeAsync(int id, UpdateEmployeeDto dto, CancellationToken ct);
    Task TerminateEmployeeAsync(int id, CancellationToken ct);
}