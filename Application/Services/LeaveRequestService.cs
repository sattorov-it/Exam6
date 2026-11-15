using Application.DTOs.LeaveRequests;
using Application.Interfaces;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class LeaveRequestService(
    ILeaveRequestRepository leaveRequestRepository,
    IEmployeeRepository employeeRepository,
    ILogger<LeaveRequestService> logger) : ILeaveRequestService
{
    private ILeaveRequestRepository repository = leaveRequestRepository;
    private IEmployeeRepository employees = employeeRepository;

    public async Task<GetLeaveRequestDto> AddLeaveRequestAsync(CreateLeaveRequestDto dto, CancellationToken ct)
    {
        var employee = await employees.GetEmployeeAsync(dto.EmployeeId, ct);
        if (employee == null)
        {
            logger.LogWarning("Employee {Id} not found", dto.EmployeeId);
            throw new NotFoundException("Employee not found.");
        }

        var start = DateTime.SpecifyKind(dto.StartDate.Date, DateTimeKind.Unspecified);
        var end = DateTime.SpecifyKind(dto.EndDate.Date, DateTimeKind.Unspecified);

        if (end < start)
        {
            logger.LogWarning("EndDate {End} is before StartDate {Start}", end, start);
            throw new ValidationException("EndDate cannot be before StartDate.");
        }

        if (start < DateTime.UtcNow.Date)
        {
            logger.LogWarning("StartDate {Start} is in the past", start);
            throw new ValidationException("StartDate cannot be in the past.");
        }

        if (dto.LeaveType == LeaveType.Vacation && (end - start).Days + 1 > 28)
        {
            logger.LogWarning("Vacation is longer than 28 days");
            throw new ValidationException("Vacation cannot be longer than 28 days.");
        }

        if (!employee.IsActive)
        {
            logger.LogWarning("Employee {Id} is terminated", employee.Id);
            throw new ConflictException("Cannot create leave request for terminated employee.");
        }

        var overlap = await repository.HasOverlappingLeaveAsync(employee.Id, start, end, ct);
        if (overlap)
        {
            logger.LogWarning("Leave request of employee {Id} overlaps with another one", employee.Id);
            throw new ConflictException("Leave request overlaps with another pending or approved request.");
        }

        var leaveRequest = new LeaveRequest()
        {
            EmployeeId = employee.Id,
            LeaveType = dto.LeaveType,
            StartDate = start,
            EndDate = end,
            Status = LeaveStatus.Pending,
            Reason = dto.Reason,
            CreatedAt = DateTime.UtcNow
        };
        leaveRequest.Id = await repository.AddLeaveRequestAsync(leaveRequest, ct);

        logger.LogInformation("Leave request {Id} of employee {EmployeeId} created", leaveRequest.Id, employee.Id);

        return new GetLeaveRequestDto()
        {
            Id = leaveRequest.Id,
            EmployeeId = employee.Id,
            EmployeeName = employee.FirstName + " " + employee.LastName,
            LeaveType = leaveRequest.LeaveType,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            Status = leaveRequest.Status,
            Reason = leaveRequest.Reason,
            CreatedAt = leaveRequest.CreatedAt
        };
    }

    public async Task<List<GetLeaveRequestDto>> GetLeaveRequestsAsync(LeaveStatus? status, CancellationToken ct)
    {
        if (status != null && !Enum.IsDefined(status.Value))
            throw new ValidationException("Unknown status.");

        return await repository.GetLeaveRequestsAsync(status, ct);
    }

    public async Task<List<GetLeaveRequestDto>> GetLeaveRequestsByEmployeeAsync(int employeeId, CancellationToken ct)
    {
        var employee = await employees.GetEmployeeAsync(employeeId, ct);
        if (employee == null)
            throw new NotFoundException("Employee not found.");

        return await repository.GetLeaveRequestsByEmployeeAsync(employeeId, ct);
    }

    public async Task ApproveLeaveRequestAsync(int id, CancellationToken ct)
    {
        await ChangeStatusAsync(id, LeaveStatus.Approved, ct);
    }

    public async Task RejectLeaveRequestAsync(int id, CancellationToken ct)
    {
        await ChangeStatusAsync(id, LeaveStatus.Rejected, ct);
    }

    private async Task ChangeStatusAsync(int id, LeaveStatus status, CancellationToken ct)
    {
        var leaveRequest = await repository.GetLeaveRequestAsync(id, ct);
        if (leaveRequest == null)
            throw new NotFoundException("Leave request not found.");

        if (leaveRequest.Status != LeaveStatus.Pending)
        {
            logger.LogWarning("Leave request {Id} is not pending, status is {Status}", id, leaveRequest.Status);
            throw new ConflictException("Only pending leave request can be approved or rejected.");
        }

        await repository.UpdateStatusAsync(id, status, ct);
        logger.LogInformation("Leave request {Id} is now {Status}", id, status);
    }
}