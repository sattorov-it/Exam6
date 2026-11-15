using Application.DTOs.Employees;
using Application.Interfaces;
using Dapper;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class EmployeeRepository(AppDbContext appDbContext) : IEmployeeRepository
{
    private AppDbContext context = appDbContext;

    public async Task<List<GetEmployeeDto>> GetEmployeesAsync(int? departmentId, int? positionId, string? search, int page, int pageSize, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = @"select e.id, e.firstname, e.lastname, e.email, e.phone, e.hiredate, e.salary, e.isactive, e.departmentid, e.positionid,
                             d.id, d.name,
                             p.id, p.title
                      from employees e
                      join departments d on d.id = e.departmentid
                      join positions p on p.id = e.positionid
                      where (@departmentId is null or e.departmentid = @departmentId)
                        and (@positionId is null or e.positionid = @positionId)
                        and (@search is null or e.firstname ilike '%' || @search || '%' or e.lastname ilike '%' || @search || '%')
                      order by e.id
                      limit @pageSize offset @offset";
        var parameters = new { departmentId, positionId, search, pageSize, offset = (page - 1) * pageSize };

        var res = await connection.QueryAsync<Employee, Department, Position, GetEmployeeDto>(
            new CommandDefinition(query, parameters, cancellationToken: ct),
            (e, d, p) => ToDto(e, d, p),
            splitOn: "id,id");
        return res.ToList();
    }

    public async Task<int> GetEmployeesCountAsync(int? departmentId, int? positionId, string? search, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = @"select count(*)
                      from employees e
                      where (@departmentId is null or e.departmentid = @departmentId)
                        and (@positionId is null or e.positionid = @positionId)
                        and (@search is null or e.firstname ilike '%' || @search || '%' or e.lastname ilike '%' || @search || '%')";
        var parameters = new { departmentId, positionId, search };
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(query, parameters, cancellationToken: ct));
    }

    public async Task<GetEmployeeDto?> GetEmployeeDtoAsync(int id, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = @"select e.id, e.firstname, e.lastname, e.email, e.phone, e.hiredate, e.salary, e.isactive, e.departmentid, e.positionid,
                             d.id, d.name,
                             p.id, p.title
                      from employees e
                      join departments d on d.id = e.departmentid
                      join positions p on p.id = e.positionid
                      where e.id = @id";

        var res = await connection.QueryAsync<Employee, Department, Position, GetEmployeeDto>(
            new CommandDefinition(query, new { id }, cancellationToken: ct),
            (e, d, p) => ToDto(e, d, p),
            splitOn: "id,id");
        return res.FirstOrDefault();
    }

    public async Task<Employee?> GetEmployeeAsync(int id, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "select * from employees where id = @id";
        return await connection.QuerySingleOrDefaultAsync<Employee>(new CommandDefinition(query, new { id }, cancellationToken: ct));
    }

    public async Task<Employee?> GetEmployeeByEmailAsync(string email, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "select * from employees where lower(email) = lower(@email)";
        return await connection.QuerySingleOrDefaultAsync<Employee>(new CommandDefinition(query, new { email }, cancellationToken: ct));
    }

    public async Task<int> AddEmployeeAsync(Employee employee, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = @"insert into employees (firstname, lastname, email, phone, hiredate, salary, isactive, departmentid, positionid)
                      values (@firstname, @lastname, @email, @phone, @hiredate, @salary, @isactive, @departmentid, @positionid)
                      returning id";
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(query, employee, cancellationToken: ct));
    }

    public async Task UpdateEmployeeAsync(Employee employee, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = @"update employees
                      set firstname = @firstname, lastname = @lastname, email = @email, phone = @phone,
                          hiredate = @hiredate, salary = @salary, departmentid = @departmentid, positionid = @positionid
                      where id = @id";
        await connection.ExecuteAsync(new CommandDefinition(query, employee, cancellationToken: ct));
    }

    public async Task TerminateEmployeeAsync(int id, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "update employees set isactive = false where id = @id";
        await connection.ExecuteAsync(new CommandDefinition(query, new { id }, cancellationToken: ct));
    }

    private static GetEmployeeDto ToDto(Employee e, Department d, Position p)
    {
        return new GetEmployeeDto()
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Phone = e.Phone,
            HireDate = e.HireDate,
            Salary = e.Salary,
            IsActive = e.IsActive,
            DepartmentId = e.DepartmentId,
            DepartmentName = d.Name,
            PositionId = e.PositionId,
            PositionTitle = p.Title
        };
    }
}