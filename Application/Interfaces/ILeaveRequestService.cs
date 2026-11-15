using Application.DTOs.LeaveRequests;
using Domain.Enums;

namespace Application.Interfaces;

public interface ILeaveRequestService
{
    Task<GetLeaveRequestDto> AddLeaveRequestAsync(CreateLeaveRequestDto dto, CancellationToken ct);
    Task<List<GetLeaveRequestDto>> GetLeaveRequestsAsync(LeaveStatus? status, CancellationToken ct);
    Task<List<GetLeaveRequestDto>> GetLeaveRequestsByEmployeeAsync(int employeeId, CancellationToken ct);
    Task ApproveLeaveRequestAsync(int id, CancellationToken ct);
    Task RejectLeaveRequestAsync(int id, CancellationToken ct);
}