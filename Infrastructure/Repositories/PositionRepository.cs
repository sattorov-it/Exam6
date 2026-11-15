using Application.Interfaces;
using Dapper;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class PositionRepository(AppDbContext appDbContext) : IPositionRepository
{
    private AppDbContext context = appDbContext;

    public async Task<List<Position>> GetPositionsAsync(CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "select * from positions order by id";
        var res = await connection.QueryAsync<Position>(new CommandDefinition(query, cancellationToken: ct));
        return res.ToList();
    }

    public async Task<Position?> GetPositionAsync(int id, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "select * from positions where id = @id";
        return await connection.QuerySingleOrDefaultAsync<Position>(new CommandDefinition(query, new { id }, cancellationToken: ct));
    }

    public async Task<Position?> GetPositionByTitleAsync(string title, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "select * from positions where lower(title) = lower(@title)";
        return await connection.QuerySingleOrDefaultAsync<Position>(new CommandDefinition(query, new { title }, cancellationToken: ct));
    }

    public async Task<int> AddPositionAsync(Position position, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "insert into positions (title, minsalary, maxsalary) values (@title, @minsalary, @maxsalary) returning id";
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(query, position, cancellationToken: ct));
    }
}