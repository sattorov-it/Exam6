using Application.DTOs;
using Application.DTOs.Employees;
using Application.DTOs.LeaveRequests;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController(IEmployeeService employeeService, ILeaveRequestService leaveRequestService) : ControllerBase
{
    private IEmployeeService service = employeeService;
    private ILeaveRequestService leaveService = leaveRequestService;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GetEmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetEmployees(int? departmentId, int? positionId, string? search, CancellationToken ct, int page = 1, int pageSize = 20)
    {
        return Ok(await service.GetEmployeesAsync(departmentId, positionId, search, page, pageSize, ct));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetEmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployee(int id, CancellationToken ct)
    {
        return Ok(await service.GetEmployeeAsync(id, ct));
    }

    [HttpPost]
    [ProducesResponseType(typeof(GetEmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddEmployee(CreateEmployeeDto dto, CancellationToken ct)
    {
        var res = await service.AddEmployeeAsync(dto, ct);
        return CreatedAtAction(nameof(GetEmployee), new { id = res.Id }, res);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto dto, CancellationToken ct)
    {
        await service.UpdateEmployeeAsync(id, dto, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/terminate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> TerminateEmployee(int id, CancellationToken ct)
    {
        await service.TerminateEmployeeAsync(id, ct);
        return NoContent();
    }

    [HttpGet("{id:int}/leave-requests")]
    [ProducesResponseType(typeof(List<GetLeaveRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployeeLeaveRequests(int id, CancellationToken ct)
    {
        return Ok(await leaveService.GetLeaveRequestsByEmployeeAsync(id, ct));
    }
}