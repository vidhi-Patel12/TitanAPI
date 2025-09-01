using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface ITimesheetEntry
    {
        Task<IEnumerable<TimesheetEntry>> GetAllAsync();
        Task<IEnumerable<TimesheetEntry>> GetByTimesheetIdAsync(int timesheetId);
        Task<TimesheetEntry?> GetByIdAsync(int entryId);
        Task<TimesheetEntry> InsertUpdateAsync(TimesheetEntry entry);
        Task<bool> DeleteAsync(int entryId);

    }
}
