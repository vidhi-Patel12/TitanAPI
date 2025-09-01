using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface ITimesheet
    {
        Task<IEnumerable<Timesheet>> GetAllAsync();
        Task<Timesheet?> GetByIdAsync(int timesheetId);
        Task<Timesheet> InsertUpdateAsync(Timesheet timesheet);
        Task<bool> DeleteAsync(int timesheetId);

        Task<Timesheet> SubmitAsync(int timesheetId, string newStatus);

        Task<IEnumerable<Timesheet>> GetReportAsync(string? projectCode, int? employeeId, string? status);

        IEnumerable<TimesheetEntry> GenerateTimesheetEntries(Timesheet timesheet);

    }
}
