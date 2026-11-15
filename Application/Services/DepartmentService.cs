using Application.DTOs.Departments;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class DepartmentService(IDepartmentRepository departmentRepository, ILogger<DepartmentService> logger) : IDepartmentService
{
    private IDepartmentRepository repository = departmentRepository;

    public async Task<List<GetDepartmentDto>> GetDepartmentsAsync(CancellationToken ct)
    {
        var departments = await repository.GetDepartmentsAsync(ct);
        return departments.Select(ToDto).ToList();
    }

    public async Task<GetDepartmentDto> GetDepartmentAsync(int id, CancellationToken ct)
    {
        var department = await repository.GetDepartmentAsync(id, ct);
        if (department == null)
            throw new NotFoundException("Department not found.");

        return ToDto(department);
    }

    public async Task<GetDepartmentDto> AddDepartmentAsync(CreateDepartmentDto dto, CancellationToken ct)
    {
        var same = await repository.GetDepartmentByNameAsync(dto.Name, ct);
        if (same != null)
        {
            logger.LogWarning("Department with name {Name} already exists", dto.Name);
            throw new ConflictException("Department with this name already exists.");
        }

        var department = new Department()
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };
        department.Id = await repository.AddDepartmentAsync(department, ct);

        logger.LogInformation("Department {Id} {Name} created", department.Id, department.Name);
        return ToDto(department);
    }

    public async Task UpdateDepartmentAsync(int id, UpdateDepartmentDto dto, CancellationToken ct)
    {
        var department = await repository.GetDepartmentAsync(id, ct);
        if (department == null)
            throw new NotFoundException("Department not found.");

        var same = await repository.GetDepartmentByNameAsync(dto.Name, ct);
        if (same != null && same.Id != id)
        {
            logger.LogWarning("Department with name {Name} already exists", dto.Name);
            throw new ConflictException("Department with this name already exists.");
        }

        department.Name = dto.Name;
        department.Description = dto.Description;
        await repository.UpdateDepartmentAsync(department, ct);

        logger.LogInformation("Department {Id} updated", id);
    }

    public async Task DeleteDepartmentAsync(int id, CancellationToken ct)
    {
        var department = await repository.GetDepartmentAsync(id, ct);
        if (department == null)
            throw new NotFoundException("Department not found.");

        var hasEmployees = await repository.HasEmployeesAsync(id, ct);
        if (hasEmployees)
        {
            logger.LogWarning("Cannot delete department {Id}: it has employees", id);
            throw new ConflictException("Cannot delete department that has employees.");
        }

        await repository.DeleteDepartmentAsync(id, ct);
        logger.LogInformation("Department {Id} deleted", id);
    }

    private static GetDepartmentDto ToDto(Department department)
    {
        return new GetDepartmentDto()
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            CreatedAt = department.CreatedAt
        };
    }
}