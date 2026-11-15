using Application.Interfaces;
using Dapper;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class DepartmentRepository(AppDbContext appDbContext) : IDepartmentRepository
{
    private AppDbContext context = appDbContext;

    public async Task<List<Department>> GetDepartmentsAsync(CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "select * from departments order by id";
        var res = await connection.QueryAsync<Department>(new CommandDefinition(query, cancellationToken: ct));
        return res.ToList();
    }

    public async Task<Department?> GetDepartmentAsync(int id, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "select * from departments where id = @id";
        return await connection.QuerySingleOrDefaultAsync<Department>(new CommandDefinition(query, new { id }, cancellationToken: ct));
    }

    public async Task<Department?> GetDepartmentByNameAsync(string name, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "select * from departments where lower(name) = lower(@name)";
        return await connection.QuerySingleOrDefaultAsync<Department>(new CommandDefinition(query, new { name }, cancellationToken: ct));
    }

    public async Task<int> AddDepartmentAsync(Department department, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "insert into departments (name, description, createdat) values (@name, @description, @createdat) returning id";
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(query, department, cancellationToken: ct));
    }

    public async Task UpdateDepartmentAsync(Department department, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "update departments set name = @name, description = @description where id = @id";
        await connection.ExecuteAsync(new CommandDefinition(query, department, cancellationToken: ct));
    }

    public async Task DeleteDepartmentAsync(int id, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "delete from departments where id = @id";
        await connection.ExecuteAsync(new CommandDefinition(query, new { id }, cancellationToken: ct));
    }

    public async Task<bool> HasEmployeesAsync(int id, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "select count(*) from employees where departmentid = @id";
        var count = await connection.ExecuteScalarAsync<int>(new CommandDefinition(query, new { id }, cancellationToken: ct));
        return count > 0;
    }
}