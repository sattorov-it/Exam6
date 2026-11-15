using Application.DTOs.Positions;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class PositionService(IPositionRepository positionRepository, ILogger<PositionService> logger) : IPositionService
{
    private IPositionRepository repository = positionRepository;

    public async Task<List<GetPositionDto>> GetPositionsAsync(CancellationToken ct)
    {
        var positions = await repository.GetPositionsAsync(ct);
        return positions.Select(ToDto).ToList();
    }

    public async Task<GetPositionDto> GetPositionAsync(int id, CancellationToken ct)
    {
        var position = await repository.GetPositionAsync(id, ct);
        if (position == null)
            throw new NotFoundException("Position not found.");

        return ToDto(position);
    }

    public async Task<GetPositionDto> AddPositionAsync(CreatePositionDto dto, CancellationToken ct)
    {
        if (dto.MaxSalary < dto.MinSalary)
        {
            logger.LogWarning("MaxSalary {Max} is less than MinSalary {Min}", dto.MaxSalary, dto.MinSalary);
            throw new ValidationException("MaxSalary cannot be less than MinSalary.");
        }

        var same = await repository.GetPositionByTitleAsync(dto.Title, ct);
        if (same != null)
        {
            logger.LogWarning("Position with title {Title} already exists", dto.Title);
            throw new ConflictException("Position with this title already exists.");
        }

        var position = new Position()
        {
            Title = dto.Title,
            MinSalary = dto.MinSalary,
            MaxSalary = dto.MaxSalary
        };
        position.Id = await repository.AddPositionAsync(position, ct);

        logger.LogInformation("Position {Id} {Title} created", position.Id, position.Title);
        return ToDto(position);
    }

    private static GetPositionDto ToDto(Position position)
    {
        return new GetPositionDto()
        {
            Id = position.Id,
            Title = position.Title,
            MinSalary = position.MinSalary,
            MaxSalary = position.MaxSalary
        };
    }
}