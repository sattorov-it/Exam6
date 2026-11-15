using Application.DTOs;
using Application.DTOs.Employees;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class EmployeeService(
    IEmployeeRepository employeeRepository,
    IDepartmentRepository departmentRepository,
    IPositionRepository positionRepository,
    ILogger<EmployeeService> logger) : IEmployeeService
{
    private IEmployeeRepository repository = employeeRepository;
    private IDepartmentRepository departments = departmentRepository;
    private IPositionRepository positions = positionRepository;

    public async Task<PagedResult<GetEmployeeDto>> GetEmployeesAsync(int? departmentId, int? positionId, string? search, int page, int pageSize, CancellationToken ct)
    {
        if (page < 1)
            throw new ValidationException("Page must be 1 or greater.");
        if (pageSize < 1 || pageSize > 100)
            throw new ValidationException("PageSize must be between 1 and 100.");

        search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

        var items = await repository.GetEmployeesAsync(departmentId, positionId, search, page, pageSize, ct);
        var totalCount = await repository.GetEmployeesCountAsync(departmentId, positionId, search, ct);

        return new PagedResult<GetEmployeeDto>()
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<GetEmployeeDto> GetEmployeeAsync(int id, CancellationToken ct)
    {
        var employee = await repository.GetEmployeeDtoAsync(id, ct);
        if (employee == null)
            throw new NotFoundException("Employee not found.");

        return employee;
    }

    public async Task<GetEmployeeDto> AddEmployeeAsync(CreateEmployeeDto dto, CancellationToken ct)
    {
        await CheckEmployeeAsync(dto.HireDate, dto.Salary, dto.DepartmentId, dto.PositionId, ct);

        var sameEmail = await repository.GetEmployeeByEmailAsync(dto.Email, ct);
        if (sameEmail != null)
        {
            logger.LogWarning("Employee with email {Email} already exists", dto.Email);
            throw new ConflictException("Employee with this email already exists.");
        }

        var employee = new Employee()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            HireDate = DateTime.SpecifyKind(dto.HireDate.Date, DateTimeKind.Unspecified),
            Salary = dto.Salary,
            IsActive = true,
            DepartmentId = dto.DepartmentId,
            PositionId = dto.PositionId
        };
        employee.Id = await repository.AddEmployeeAsync(employee, ct);

        logger.LogInformation("Employee {Id} {Email} hired", employee.Id, employee.Email);

        var res = await repository.GetEmployeeDtoAsync(employee.Id, ct);
        return res!;
    }

    public async Task UpdateEmployeeAsync(int id, UpdateEmployeeDto dto, CancellationToken ct)
    {
        var employee = await repository.GetEmployeeAsync(id, ct);
        if (employee == null)
            throw new NotFoundException("Employee not found.");

        await CheckEmployeeAsync(dto.HireDate, dto.Salary, dto.DepartmentId, dto.PositionId, ct);

        var sameEmail = await repository.GetEmployeeByEmailAsync(dto.Email, ct);
        if (sameEmail != null && sameEmail.Id != id)
        {
            logger.LogWarning("Employee with email {Email} already exists", dto.Email);
            throw new ConflictException("Employee with this email already exists.");
        }

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.Phone = dto.Phone;
        employee.HireDate = DateTime.SpecifyKind(dto.HireDate.Date, DateTimeKind.Unspecified);
        employee.Salary = dto.Salary;
        employee.DepartmentId = dto.DepartmentId;
        employee.PositionId = dto.PositionId;
        await repository.UpdateEmployeeAsync(employee, ct);

        logger.LogInformation("Employee {Id} updated", id);
    }

    public async Task TerminateEmployeeAsync(int id, CancellationToken ct)
    {
        var employee = await repository.GetEmployeeAsync(id, ct);
        if (employee == null)
            throw new NotFoundException("Employee not found.");

        if (!employee.IsActive)
        {
            logger.LogWarning("Employee {Id} is already terminated", id);
            throw new ConflictException("Employee is already terminated.");
        }

        await repository.TerminateEmployeeAsync(id, ct);
        logger.LogInformation("Employee {Id} terminated", id);
    }

    private async Task CheckEmployeeAsync(DateTime hireDate, decimal salary, int departmentId, int positionId, CancellationToken ct)
    {
        if (hireDate.Date > DateTime.UtcNow.Date)
        {
            logger.LogWarning("Hire date {HireDate} is in the future", hireDate);
            throw new ValidationException("Hire date cannot be in the future.");
        }

        var department = await departments.GetDepartmentAsync(departmentId, ct);
        if (department == null)
        {
            logger.LogWarning("Department {Id} not found", departmentId);
            throw new NotFoundException("Department not found.");
        }

        var position = await positions.GetPositionAsync(positionId, ct);
        if (position == null)
        {
            logger.LogWarning("Position {Id} not found", positionId);
            throw new NotFoundException("Position not found.");
        }

        if (salary < position.MinSalary || salary > position.MaxSalary)
        {
            logger.LogWarning("Salary {Salary} is out of range of position {Id}", salary, positionId);
            throw new ValidationException($"Salary must be between {position.MinSalary} and {position.MaxSalary}.");
        }
    }
}