using Application.DTOs.Departments;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController(IDepartmentService departmentService) : ControllerBase
{
    private IDepartmentService service = departmentService;

    [HttpGet]
    [ProducesResponseType(typeof(List<GetDepartmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartments(CancellationToken ct)
    {
        return Ok(await service.GetDepartmentsAsync(ct));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetDepartmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDepartment(int id, CancellationToken ct)
    {
        return Ok(await service.GetDepartmentAsync(id, ct));
    }

    [HttpPost]
    [ProducesResponseType(typeof(GetDepartmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddDepartment(CreateDepartmentDto dto, CancellationToken ct)
    {
        var res = await service.AddDepartmentAsync(dto, ct);
        return CreatedAtAction(nameof(GetDepartment), new { id = res.Id }, res);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateDepartment(int id, UpdateDepartmentDto dto, CancellationToken ct)
    {
        await service.UpdateDepartmentAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteDepartment(int id, CancellationToken ct)
    {
        await service.DeleteDepartmentAsync(id, ct);
        return NoContent();
    }
}