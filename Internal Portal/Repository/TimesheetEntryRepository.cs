using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class TimesheetEntryRepository : ITimesheetEntry
    {
        private readonly ISqlConnectionFactory _factory;
        public TimesheetEntryRepository(ISqlConnectionFactory factory) => _factory = factory;

        public async Task<IEnumerable<TimesheetEntry>> GetAllAsync()
        {
            var results = new List<TimesheetEntry>();
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.TimesheetEntry_GetAll";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapEntry(reader));
            }
            return results;
        }

        public async Task<IEnumerable<TimesheetEntry>> GetByTimesheetIdAsync(int timesheetId)
        {
            var results = new List<TimesheetEntry>();
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.TimesheetEntry_GetByTimesheetId";
            cmd.Parameters.Add(new SqlParameter("@TimesheetId", SqlDbType.Int) { Value = timesheetId });

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapEntry(reader));
            }
            return results;
        }

        public async Task<TimesheetEntry?> GetByIdAsync(int entryId)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.TimesheetEntry_GetById";
            cmd.Parameters.Add(new SqlParameter("@EntryId", SqlDbType.Int) { Value = entryId });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapEntry(reader);
            return null;
        }

        public async Task<TimesheetEntry> InsertUpdateAsync(TimesheetEntry entry)
        {
            if (entry is null) throw new ArgumentNullException(nameof(entry));

            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.TimesheetEntry_InsertUpdate";

            cmd.Parameters.Add(new SqlParameter("@EntryId", SqlDbType.Int) { Value = (object?)entry.EntryId ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@TimesheetId", SqlDbType.Int) { Value = entry.TimesheetId });
            cmd.Parameters.Add(new SqlParameter("@EntryDate", SqlDbType.Date) { Value = (object?)entry.EntryDate ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@DayName", SqlDbType.VarChar, 20) { Value = (object?)entry.DayName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@HoursWorked", SqlDbType.Decimal) { Value = (object?)entry.HoursWorked ?? DBNull.Value, Precision = 5, Scale = 2 });
            cmd.Parameters.Add(new SqlParameter("@ShortDescription", SqlDbType.VarChar, 255) { Value = (object?)entry.ShortDescription ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Location", SqlDbType.VarChar, 100) { Value = (object?)entry.Location ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Attendance", SqlDbType.VarChar, 20) { Value = (object?)entry.Attendance ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ExtraDay", SqlDbType.VarChar, 20) { Value = (object?)entry.ExtraDay ?? DBNull.Value });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapEntry(reader);

            throw new InvalidOperationException("Merge did not return the saved TimesheetEntry row.");
        }

        public async Task<bool> DeleteAsync(int entryId)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.TimesheetEntry_Delete";
            cmd.Parameters.Add(new SqlParameter("@EntryId", SqlDbType.Int) { Value = entryId });

            var result = await cmd.ExecuteScalarAsync();
            return result != null && Convert.ToInt32(result) > 0;
        }

        private static TimesheetEntry MapEntry(SqlDataReader reader)
        {
            return new TimesheetEntry
            {
                EntryId = Convert.ToInt32(reader["entry_id"]),
                TimesheetId = Convert.ToInt32(reader["timesheet_id"]),
                EntryDate = reader["entry_date"] as DateTime?,
                DayName = reader["day_name"] as string,
                HoursWorked = reader["hours_worked"] as decimal?,
                ShortDescription = reader["short_description"] as string,
                Location = reader["location"] as string,
                Attendance = reader["attendance"] as string,
                ExtraDay = reader["extra_day"] as string
            };
        }
    }
}