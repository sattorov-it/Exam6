using Application.DTOs.LeaveRequests;
using Domain.Enums;
using Domain.Models;

namespace Application.Interfaces;

public interface ILeaveRequestRepository
{
    Task<int> AddLeaveRequestAsync(LeaveRequest leaveRequest, CancellationToken ct);
    Task<LeaveRequest?> GetLeaveRequestAsync(int id, CancellationToken ct);
    Task<List<GetLeaveRequestDto>> GetLeaveRequestsAsync(LeaveStatus? status, CancellationToken ct);
    Task<List<GetLeaveRequestDto>> GetLeaveRequestsByEmployeeAsync(int employeeId, CancellationToken ct);
    Task<bool> HasOverlappingLeaveAsync(int employeeId, DateTime startDate, DateTime endDate, CancellationToken ct);
    Task UpdateStatusAsync(int id, LeaveStatus status, CancellationToken ct);
}