using Domain.Models;

namespace Application.Interfaces;

public interface IPositionRepository
{
    Task<List<Position>> GetPositionsAsync(CancellationToken ct);
    Task<Position?> GetPositionAsync(int id, CancellationToken ct);
    Task<Position?> GetPositionByTitleAsync(string title, CancellationToken ct);
    Task<int> AddPositionAsync(Position position, CancellationToken ct);
}