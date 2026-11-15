using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Data;

public class AppDbContext(IConfiguration configuration)
{
    private string connectionString = configuration.GetConnectionString("DefaultConnection")!;

    public IDbConnection Connection() => new NpgsqlConnection(connectionString);
}