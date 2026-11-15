using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.LeaveRequests;

public class CreateLeaveRequestDto
{
    [Range(1, int.MaxValue)]
    public int EmployeeId { get; set; }

    [EnumDataType(typeof(LeaveType))]
    public LeaveType LeaveType { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }
}