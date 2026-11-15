using Application.DTOs.Positions;

namespace Application.Interfaces;

public interface IPositionService
{
    Task<List<GetPositionDto>> GetPositionsAsync(CancellationToken ct);
    Task<GetPositionDto> GetPositionAsync(int id, CancellationToken ct);
    Task<GetPositionDto> AddPositionAsync(CreatePositionDto dto, CancellationToken ct);
}