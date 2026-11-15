using Application.DTOs.LeaveRequests;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/leave-requests")]
public class LeaveRequestsController(ILeaveRequestService leaveRequestService) : ControllerBase
{
    private ILeaveRequestService service = leaveRequestService;

    [HttpPost]
    [ProducesResponseType(typeof(GetLeaveRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddLeaveRequest(CreateLeaveRequestDto dto, CancellationToken ct)
    {
        var res = await service.AddLeaveRequestAsync(dto, ct);
        return Created($"/api/employees/{res.EmployeeId}/leave-requests", res);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<GetLeaveRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLeaveRequests(LeaveStatus? status, CancellationToken ct)
    {
        return Ok(await service.GetLeaveRequestsAsync(status, ct));
    }

    [HttpPost("{id:int}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveLeaveRequest(int id, CancellationToken ct)
    {
        await service.ApproveLeaveRequestAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RejectLeaveRequest(int id, CancellationToken ct)
    {
        await service.RejectLeaveRequestAsync(id, ct);
        return NoContent();
    }
}