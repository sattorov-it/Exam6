using Application.DTOs.LeaveRequests;
using Application.Interfaces;
using Dapper;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class LeaveRequestRepository(AppDbContext appDbContext) : ILeaveRequestRepository
{
    private AppDbContext context = appDbContext;

    public async Task<int> AddLeaveRequestAsync(LeaveRequest leaveRequest, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = @"insert into leaverequests (employeeid, leavetype, startdate, enddate, status, reason, createdat)
                      values (@employeeId, @leaveType, @startDate, @endDate, @status, @reason, @createdAt)
                      returning id";
        var parameters = new
        {
            employeeId = leaveRequest.EmployeeId,
            leaveType = (int)leaveRequest.LeaveType,
            startDate = leaveRequest.StartDate,
            endDate = leaveRequest.EndDate,
            status = (int)leaveRequest.Status,
            reason = leaveRequest.Reason,
            createdAt = leaveRequest.CreatedAt
        };
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(query, parameters, cancellationToken: ct));
    }

    public async Task<LeaveRequest?> GetLeaveRequestAsync(int id, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "select * from leaverequests where id = @id";
        return await connection.QuerySingleOrDefaultAsync<LeaveRequest>(new CommandDefinition(query, new { id }, cancellationToken: ct));
    }

    public async Task<List<GetLeaveRequestDto>> GetLeaveRequestsAsync(LeaveStatus? status, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = @"select l.id, l.employeeid, l.leavetype, l.startdate, l.enddate, l.status, l.reason, l.createdat,
                             e.id, e.firstname, e.lastname
                      from leaverequests l
                      join employees e on e.id = l.employeeid
                      where (@status is null or l.status = @status)
                      order by l.id";
        var parameters = new { status = (int?)status };

        var res = await connection.QueryAsync<LeaveRequest, Employee, GetLeaveRequestDto>(
            new CommandDefinition(query, parameters, cancellationToken: ct),
            (l, e) => ToDto(l, e),
            splitOn: "id");
        return res.ToList();
    }

    public async Task<List<GetLeaveRequestDto>> GetLeaveRequestsByEmployeeAsync(int employeeId, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = @"select l.id, l.employeeid, l.leavetype, l.startdate, l.enddate, l.status, l.reason, l.createdat,
                             e.id, e.firstname, e.lastname
                      from leaverequests l
                      join employees e on e.id = l.employeeid
                      where l.employeeid = @employeeId
                      order by l.id";

        var res = await connection.QueryAsync<LeaveRequest, Employee, GetLeaveRequestDto>(
            new CommandDefinition(query, new { employeeId }, cancellationToken: ct),
            (l, e) => ToDto(l, e),
            splitOn: "id");
        return res.ToList();
    }

    public async Task<bool> HasOverlappingLeaveAsync(int employeeId, DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = @"select count(*)
                      from leaverequests
                      where employeeid = @employeeId
                        and status in (1, 2)
                        and startdate <= @endDate
                        and enddate >= @startDate";
        var count = await connection.ExecuteScalarAsync<int>(new CommandDefinition(query, new { employeeId, startDate, endDate }, cancellationToken: ct));
        return count > 0;
    }

    public async Task UpdateStatusAsync(int id, LeaveStatus status, CancellationToken ct)
    {
        using var connection = context.Connection();
        var query = "update leaverequests set status = @status where id = @id";
        await connection.ExecuteAsync(new CommandDefinition(query, new { id, status = (int)status }, cancellationToken: ct));
    }

    private static GetLeaveRequestDto ToDto(LeaveRequest l, Employee e)
    {
        return new GetLeaveRequestDto()
        {
            Id = l.Id,
            EmployeeId = l.EmployeeId,
            EmployeeName = e.FirstName + " " + e.LastName,
            LeaveType = l.LeaveType,
            StartDate = l.StartDate,
            EndDate = l.EndDate,
            Status = l.Status,
            Reason = l.Reason,
            CreatedAt = l.CreatedAt
        };
    }
}