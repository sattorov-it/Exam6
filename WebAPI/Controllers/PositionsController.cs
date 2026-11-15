using Application.DTOs.Positions;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/positions")]
public class PositionsController(IPositionService positionService) : ControllerBase
{
    private IPositionService service = positionService;

    [HttpGet]
    [ProducesResponseType(typeof(List<GetPositionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPositions(CancellationToken ct)
    {
        return Ok(await service.GetPositionsAsync(ct));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetPositionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPosition(int id, CancellationToken ct)
    {
        return Ok(await service.GetPositionAsync(id, ct));
    }

    [HttpPost]
    [ProducesResponseType(typeof(GetPositionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddPosition(CreatePositionDto dto, CancellationToken ct)
    {
        var res = await service.AddPositionAsync(dto, ct);
        return CreatedAtAction(nameof(GetPosition), new { id = res.Id }, res);
    }
}