using Application.DTOs.Departments;

namespace Application.Interfaces;

public interface IDepartmentService
{
    Task<List<GetDepartmentDto>> GetDepartmentsAsync(CancellationToken ct);
    Task<GetDepartmentDto> GetDepartmentAsync(int id, CancellationToken ct);
    Task<GetDepartmentDto> AddDepartmentAsync(CreateDepartmentDto dto, CancellationToken ct);
    Task UpdateDepartmentAsync(int id, UpdateDepartmentDto dto, CancellationToken ct);
    Task DeleteDepartmentAsync(int id, CancellationToken ct);
}