using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class TimesheetRepository : ITimesheet
    {
        private readonly ISqlConnectionFactory _factory;
        public TimesheetRepository(ISqlConnectionFactory factory) => _factory = factory;

        public async Task<IEnumerable<Timesheet>> GetAllAsync()
        {
            var results = new List<Timesheet>();
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.Timesheet_GetAll";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapTimesheet(reader));
            }
            return results;
        }

        public async Task<Timesheet?> GetByIdAsync(int timesheetId)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.Timesheet_GetById";
            cmd.Parameters.Add(new SqlParameter("@TimesheetId", SqlDbType.Int) { Value = timesheetId });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapTimesheet(reader);
            return null;
        }

        public async Task<Timesheet> InsertUpdateAsync(Timesheet m)
        {
            if (m is null) throw new ArgumentNullException(nameof(m));

            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.Timesheet_InsertUpdate";

            cmd.Parameters.Add(new SqlParameter("@TimesheetId", SqlDbType.Int)
            {
                Value = m.TimesheetId == 0 ? DBNull.Value : m.TimesheetId
            });
            cmd.Parameters.Add(new SqlParameter("@ProjectCode", SqlDbType.VarChar, 20) { Value = (object?)m.ProjectCode ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = (object?)m.EmployeeId ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@TimesheetType", SqlDbType.VarChar, 20) { Value = (object?)m.TimesheetType ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@MonthYear", SqlDbType.VarChar, 20) { Value = (object?)m.MonthYear ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.Date) { Value = (object?)m.StartDate ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.Date) { Value = (object?)m.EndDate ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 20) { Value = (object?)m.Status ?? DBNull.Value });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapTimesheet(reader);

            throw new InvalidOperationException("Merge did not return the saved Timesheet row.");
        }

        public async Task<bool> DeleteAsync(int timesheetId)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.Timesheet_Delete";
            cmd.Parameters.Add(new SqlParameter("@TimesheetId", SqlDbType.Int) { Value = timesheetId });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Convert.ToInt32(reader["RowsAffected"]) > 0;
            }
            return false;
        }

        public async Task<Timesheet> SubmitAsync(int timesheetId, string newStatus)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.Timesheet_Submit";
            cmd.Parameters.Add(new SqlParameter("@TimesheetId", SqlDbType.Int) { Value = timesheetId });
            cmd.Parameters.Add(new SqlParameter("@NewStatus", SqlDbType.VarChar, 50) { Value = newStatus });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapTimesheet(reader);

            throw new InvalidOperationException("Timesheet submit failed.");
        }

        public async Task<IEnumerable<Timesheet>> GetReportAsync(string? projectCode, int? employeeId, string? status)
        {
            var results = new List<Timesheet>();
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.Timesheet_Report";

            cmd.Parameters.Add(new SqlParameter("@ProjectCode", SqlDbType.VarChar, 20) { Value = (object?)projectCode ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = (object?)employeeId ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 50) { Value = (object?)status ?? DBNull.Value });

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapTimesheet(reader));
            }
            return results;
        }

        // Generate Entries (Monthly, Quarterly, Fixed)
        public IEnumerable<TimesheetEntry> GenerateTimesheetEntries(Timesheet timesheet)
        {
            var entries = new List<TimesheetEntry>();
            if (timesheet.StartDate == null || timesheet.EndDate == null) return entries;

            DateTime current = timesheet.StartDate.Value;
            while (current <= timesheet.EndDate.Value)
            {
                entries.Add(new TimesheetEntry
                {
                    TimesheetId = timesheet.TimesheetId,
                    EntryDate = current,
                    DayName = current.DayOfWeek.ToString(),
                    HoursWorked = 0,
                    Attendance = "Working"
                });
                current = current.AddDays(1);
            }
            return entries;
        }

        private static Timesheet MapTimesheet(SqlDataReader reader)
        {
            return new Timesheet
            {
                TimesheetId = Convert.ToInt32(reader["timesheet_id"]),
                ProjectCode = reader["project_code"] as string,
                EmployeeId = Convert.ToInt32(reader["employee_id"]),
                TimesheetType = reader["timesheet_type"] as string,
                MonthYear = reader["month_year"] as string,
                StartDate = reader["start_date"] as DateTime?,
                EndDate = reader["end_date"] as DateTime?,
                Status = reader["status"] as string
            };
        }
    }
}
