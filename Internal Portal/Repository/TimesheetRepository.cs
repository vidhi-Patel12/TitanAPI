using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
                results.Add(MapInsertTimesheet(reader));
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
            if (await reader.ReadAsync()) return MapInsertTimesheet(reader);
            return null;
        }

        public async Task<List<Timesheet>> GetByProjectCodeWorkMonthAsync(string projectCode, DateOnly workMonth, int employeeid)
        {
            var timesheets = new List<Timesheet>();

            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.Timesheet_GetByProjectCodeWorkMonth";

            cmd.Parameters.Add(new SqlParameter("@ProjectCode", SqlDbType.VarChar, 20) { Value = projectCode });
            cmd.Parameters.Add(new SqlParameter("@WorkMonth", SqlDbType.Date) { Value = workMonth });
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = employeeid });

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                timesheets.Add(MapInsertTimesheet(reader));
            }

            return timesheets;
        }




        public async Task<IEnumerable<Timesheet>> GetByMonthAsync(int year, int month)
        {
            var results = new List<Timesheet>();

            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.Timesheet_GetByMonth";

            cmd.Parameters.Add(new SqlParameter("@Year", SqlDbType.Int) { Value = year });
            cmd.Parameters.Add(new SqlParameter("@Month", SqlDbType.Int) { Value = month });

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(MapTimesheet(reader));
            }

            return results;
        }

        public async Task<Timesheet> InsertUpdateAsync(Timesheet timesheet)
        {
            if (timesheet == null)
                throw new ArgumentNullException(nameof(timesheet));

            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.Timesheet_InsertUpdate";

            cmd.Parameters.Add(new SqlParameter("@TimesheetId", SqlDbType.Int) { Value = (object?)timesheet.TimesheetId ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ProjectCode", SqlDbType.VarChar, 20) { Value = (object?)timesheet.ProjectCode ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ProjectName", SqlDbType.NVarChar, -1) { Value = (object?)timesheet.ProjectName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = timesheet.EmployeeId });
            cmd.Parameters.Add(new SqlParameter("@WorkMonth", SqlDbType.Date) { Value = timesheet.WorkMonth });

            cmd.Parameters.Add(new SqlParameter("@TimesheetType", SqlDbType.VarChar, 20) { Value = (object?)timesheet.TimesheetType ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Rate", SqlDbType.Int) { Value = (object?)timesheet.rate ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Unit", SqlDbType.VarChar, 50) { Value = (object?)timesheet.unit ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@MonthlyWorkUnit", SqlDbType.Int) { Value = (object?)timesheet.monthlyworkunit ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Holiday", SqlDbType.Int) { Value = (object?)timesheet.holiday ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Leave", SqlDbType.Int) { Value = (object?)timesheet.leave ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ExtraDays", SqlDbType.Int) { Value = (object?)timesheet.extradays ?? DBNull.Value });

            cmd.Parameters.Add(new SqlParameter("@ActualWorkDaysHours", SqlDbType.Int) { Value = (object?)timesheet.actualworkdayshours ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@UploadTimesheet", SqlDbType.NVarChar, -1) { Value = (object?)timesheet.uploadtimesheet ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 50) { Value = (object?)timesheet.status ?? "Pending" });
            cmd.Parameters.Add(new SqlParameter("@ApprovedWorkingUnit", SqlDbType.Int) { Value = (object?)timesheet.approvedworkingunit ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@NetRate", SqlDbType.Float) { Value = (object?)timesheet.netrate ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@TdsApplicable", SqlDbType.Int) { Value = (object?)timesheet.tdsapplicable ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@NetPayable", SqlDbType.Float) { Value = (object?)timesheet.netpayble ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@NetPayableAfterTds", SqlDbType.Float) { Value = (object?)timesheet.netpaybleaftertds ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@SalaryPaid", SqlDbType.VarChar, 50) { Value = (object?)timesheet.salarypaid ?? "Unpaid" });
            cmd.Parameters.Add(new SqlParameter("@SalaryDate", SqlDbType.Date) { Value = (object?)timesheet.salarydate?.ToDateTime(TimeOnly.MinValue) ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@TdaValue", SqlDbType.Float) { Value = (object?)timesheet.tdavalue ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@TdsPaid", SqlDbType.VarChar, 50) { Value = (object?)timesheet.tdspaid ?? "Unpaid" });
            cmd.Parameters.Add(new SqlParameter("@TdsDate", SqlDbType.Date) { Value = (object?)timesheet.tdsdate?.ToDateTime(TimeOnly.MinValue) ?? DBNull.Value });

            try
            {

                // Execute and return row
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return MapInsertTimesheet(reader);
                }

                throw new InvalidOperationException("No timesheet row returned from SP.");
            }
            catch (SqlException ex)
            {
                // capture and inspect ex.Message and ex.Number
                Console.WriteLine($"SQL ERROR {ex.Number}: {ex.Message}");
                throw; // rethrow or log properly
            }
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


        public async Task<IEnumerable<Timesheet>> GetTimesheetReportAsync(int? employeeId = null, DateTime? workMonth = null, string timesheetType = null)
        {
            var results = new List<Timesheet>();
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.GetTimesheetReport";

            var pEmp = cmd.CreateParameter();
            pEmp.ParameterName = "@EmployeeId";
            pEmp.Value = (object?)employeeId ?? DBNull.Value;
            cmd.Parameters.Add(pEmp);

            var paramWorkMonth = cmd.CreateParameter();
            paramWorkMonth.ParameterName = "@WorkMonth";
            paramWorkMonth.Value = (object?)workMonth ?? DBNull.Value;
            cmd.Parameters.Add(paramWorkMonth);

            var paramTimesheetType = cmd.CreateParameter();
            paramTimesheetType.ParameterName = "@TimesheetType";
            paramTimesheetType.Value = (object?)timesheetType ?? DBNull.Value;
            cmd.Parameters.Add(paramTimesheetType);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapTimesheetReport(reader));
            }

            return results;
        }

        // Generate Entries (Monthly, Quarterly, Fixed)
        //public IEnumerable<TimesheetEntry> GenerateTimesheetEntries(Timesheet timesheet)
        //{
        //    var entries = new List<TimesheetEntry>();
        //    if (timesheet.StartDate == null || timesheet.EndDate == null) return entries;

        //    DateTime current = timesheet.StartDate.Value;
        //    while (current <= timesheet.EndDate.Value)
        //    {
        //        entries.Add(new TimesheetEntry
        //        {
        //            TimesheetId = timesheet.TimesheetId,
        //            EntryDate = current,
        //            DayName = current.DayOfWeek.ToString(),
        //            HoursWorked = 0,
        //            Attendance = "Working"
        //        });
        //        current = current.AddDays(1);
        //    }
        //    return entries;
        //}

        public static bool HasColumn(IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }


        public static Timesheet MapTimesheet(SqlDataReader reader)
        {
            return new Timesheet
            {
                TimesheetId = reader["timesheet_id"] != DBNull.Value ? Convert.ToInt32(reader["timesheet_id"]) : 0,
                ProjectCode = reader["project_code"] as string,
                ProjectName = reader["project_name"] as string,
                EmployeeId = reader["employee_id"] != DBNull.Value ? Convert.ToInt32(reader["employee_id"]) : 0,
                TimesheetType = reader["timesheet_type"] as string,
                rate = reader["rate"] != DBNull.Value ? Convert.ToInt32(reader["rate"]) : (int?)null,
                unit = reader["unit"] as string,
                monthlyworkunit = reader["monthlyworkunit"] != DBNull.Value ? Convert.ToInt32(reader["monthlyworkunit"]) : (int?)null,
                holiday = reader["holiday"] != DBNull.Value ? Convert.ToInt32(reader["holiday"]) : (int?)null,
                leave = reader["leave"] != DBNull.Value ? Convert.ToInt32(reader["leave"]) : (int?)null,
                extradays = reader["extradays"] != DBNull.Value ? Convert.ToInt32(reader["extradays"]) : (int?)null,

                actualworkdayshours = reader["actualworkdayshours"] != DBNull.Value ? Convert.ToInt32(reader["actualworkdayshours"]) : (int?)null,
                uploadtimesheet = reader["uploadtimesheet"] as string,
                status = reader["status"] as string,
                approvedworkingunit = reader["approvedworkingunit"] != DBNull.Value ? Convert.ToInt32(reader["approvedworkingunit"]) : (int?)null,
                netrate = reader["netrate"] != DBNull.Value ? Convert.ToDouble(reader["netrate"]) : (double?)null,
                tdsapplicable = reader["tdsapplicable"] != DBNull.Value ? Convert.ToInt32(reader["tdsapplicable"]) : (int?)null,
                netpayble = reader["netpayble"] != DBNull.Value ? Convert.ToDouble(reader["netpayble"]) : (double?)null,
                netpaybleaftertds = reader["netpaybleaftertds"] != DBNull.Value ? Convert.ToDouble(reader["netpaybleaftertds"]) : (double?)null,

                // salarypaid is BIT in SQL → convert safely to string (“True” / “False”)
                salarypaid = reader["salarypaid"] != DBNull.Value ? reader["salarypaid"].ToString() : null,

                salarydate = reader["salarydate"] != DBNull.Value
                    ? DateOnly.FromDateTime(Convert.ToDateTime(reader["salarydate"]))
                    : (DateOnly?)null,

                tdavalue = reader["tdavalue"] != DBNull.Value ? Convert.ToDouble(reader["tdavalue"]) : (double?)null,
                tdspaid = reader["tdspaid"] != DBNull.Value ? reader["tdspaid"].ToString() : null,

                tdsdate = reader["tdsdate"] != DBNull.Value
                    ? DateOnly.FromDateTime(Convert.ToDateTime(reader["tdsdate"]))
                    : (DateOnly?)null,

                FilterMonthStart = reader["Filter_Month_Start"] != DBNull.Value
                    ? Convert.ToDateTime(reader["Filter_Month_Start"])
                    : (DateTime?)null,

                FilterMonthEnd = reader["Filter_Month_End"] != DBNull.Value
                    ? Convert.ToDateTime(reader["Filter_Month_End"])
                    : (DateTime?)null
            };
        }

        public static Timesheet MapInsertTimesheet(SqlDataReader reader)
        {
            return new Timesheet
            {
                TimesheetId = reader["timesheet_id"] != DBNull.Value ? Convert.ToInt32(reader["timesheet_id"]) : 0,
                ProjectCode = reader["project_code"] as string,
                ProjectName = reader["project_name"] as string,
                EmployeeId = reader["employee_id"] != DBNull.Value ? Convert.ToInt32(reader["employee_id"]) : 0,
                WorkMonth = reader["WorkMonth"] != DBNull.Value
                    ? DateOnly.FromDateTime(Convert.ToDateTime(reader["WorkMonth"]))
                    : (DateOnly?)null,
                TimesheetType = reader["timesheet_type"] as string,
                rate = reader["rate"] != DBNull.Value ? Convert.ToInt32(reader["rate"]) : (int?)null,
                unit = reader["unit"] as string,
                monthlyworkunit = reader["monthlyworkunit"] != DBNull.Value ? Convert.ToInt32(reader["monthlyworkunit"]) : (int?)null,
                holiday = reader["holiday"] != DBNull.Value ? Convert.ToInt32(reader["holiday"]) : (int?)null,
                leave = reader["leave"] != DBNull.Value ? Convert.ToInt32(reader["leave"]) : (int?)null,
                extradays = reader["extradays"] != DBNull.Value ? Convert.ToInt32(reader["extradays"]) : (int?)null,
                actualworkdayshours = reader["actualworkdayshours"] != DBNull.Value ? Convert.ToInt32(reader["actualworkdayshours"]) : (int?)null,
                uploadtimesheet = reader["uploadtimesheet"] as string,
                status = reader["status"] as string,
                approvedworkingunit = reader["approvedworkingunit"] != DBNull.Value ? Convert.ToInt32(reader["approvedworkingunit"]) : (int?)null,
                netrate = reader["netrate"] != DBNull.Value ? Convert.ToDouble(reader["netrate"]) : (double?)null,
                tdsapplicable = reader["tdsapplicable"] != DBNull.Value ? Convert.ToInt32(reader["tdsapplicable"]) : (int?)null,
                netpayble = reader["netpayble"] != DBNull.Value ? Convert.ToDouble(reader["netpayble"]) : (double?)null,
                netpaybleaftertds = reader["netpaybleaftertds"] != DBNull.Value ? Convert.ToDouble(reader["netpaybleaftertds"]) : (double?)null,

                // salarypaid is BIT in SQL → convert safely to string (“True” / “False”)
                salarypaid = reader["salarypaid"] != DBNull.Value ? reader["salarypaid"].ToString() : null,

                salarydate = reader["salarydate"] != DBNull.Value
                    ? DateOnly.FromDateTime(Convert.ToDateTime(reader["salarydate"]))
                    : (DateOnly?)null,

                tdavalue = reader["tdavalue"] != DBNull.Value ? Convert.ToDouble(reader["tdavalue"]) : (double?)null,
                tdspaid = reader["tdspaid"] != DBNull.Value ? reader["tdspaid"].ToString() : null,

                tdsdate = reader["tdsdate"] != DBNull.Value
                    ? DateOnly.FromDateTime(Convert.ToDateTime(reader["tdsdate"]))
                    : (DateOnly?)null,

                //FilterMonthStart = reader["Filter_Month_Start"] != DBNull.Value
                //    ? Convert.ToDateTime(reader["Filter_Month_Start"])
                //    : (DateTime?)null,

                //FilterMonthEnd = reader["Filter_Month_End"] != DBNull.Value
                //    ? Convert.ToDateTime(reader["Filter_Month_End"])
                //    : (DateTime?)null
            };

        }

        private Timesheet MapTimesheetReport(IDataReader reader)
        {
            return new Timesheet
            {
                TimesheetId = reader.GetInt32(reader.GetOrdinal("timesheet_id")),
                ProjectCode = reader.GetString(reader.GetOrdinal("project_code")),
                ProjectName = reader.GetString(reader.GetOrdinal("project_name")),
                EmployeeId = reader.GetInt32(reader.GetOrdinal("employee_id")),
                EmployeeName = reader.GetString(reader.GetOrdinal("employee_name")),
                EmployeeEmail = reader.GetString(reader.GetOrdinal("employee_email")),
                WorkMonth = reader.IsDBNull(reader.GetOrdinal("WorkMonth"))
                    ? null
                    : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("WorkMonth"))),
                TimesheetType = reader.GetString(reader.GetOrdinal("timesheet_type")),
                rate = reader.IsDBNull(reader.GetOrdinal("rate")) ? null : reader.GetInt32(reader.GetOrdinal("rate")),
                unit = reader.GetString(reader.GetOrdinal("unit")),
                monthlyworkunit = reader.IsDBNull(reader.GetOrdinal("monthlyworkunit")) ? null : reader.GetInt32(reader.GetOrdinal("monthlyworkunit")),
                holiday = reader.IsDBNull(reader.GetOrdinal("holiday")) ? null : reader.GetInt32(reader.GetOrdinal("holiday")),
                leave = reader.IsDBNull(reader.GetOrdinal("leave")) ? null : reader.GetInt32(reader.GetOrdinal("leave")),
                extradays = reader.IsDBNull(reader.GetOrdinal("extradays")) ? null : reader.GetInt32(reader.GetOrdinal("extradays")),
                actualworkdayshours = reader.IsDBNull(reader.GetOrdinal("actualworkdayshours")) ? null : reader.GetInt32(reader.GetOrdinal("actualworkdayshours")),
                uploadtimesheet = reader.IsDBNull(reader.GetOrdinal("uploadtimesheet")) ? null : reader.GetString(reader.GetOrdinal("uploadtimesheet")),
                status = reader.IsDBNull(reader.GetOrdinal("status")) ? null : reader.GetString(reader.GetOrdinal("status")),
                approvedworkingunit = reader.IsDBNull(reader.GetOrdinal("approvedworkingunit")) ? null : reader.GetInt32(reader.GetOrdinal("approvedworkingunit")),
                netrate = reader.IsDBNull(reader.GetOrdinal("netrate")) ? null : reader.GetDouble(reader.GetOrdinal("netrate")),
                tdsapplicable = reader.IsDBNull(reader.GetOrdinal("tdsapplicable")) ? null : reader.GetInt32(reader.GetOrdinal("tdsapplicable")),
                netpayble = reader.IsDBNull(reader.GetOrdinal("netpayble")) ? null : reader.GetDouble(reader.GetOrdinal("netpayble")),
                netpaybleaftertds = reader.IsDBNull(reader.GetOrdinal("netpaybleaftertds")) ? null : reader.GetDouble(reader.GetOrdinal("netpaybleaftertds")),
                salarypaid = reader.IsDBNull(reader.GetOrdinal("salarypaid")) ? null : reader.GetString(reader.GetOrdinal("salarypaid")),
                salarydate = reader.IsDBNull(reader.GetOrdinal("salarydate")) ? null : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("salarydate"))),
                tdavalue = reader.IsDBNull(reader.GetOrdinal("tdavalue")) ? null : reader.GetDouble(reader.GetOrdinal("tdavalue")),
                tdspaid = reader.IsDBNull(reader.GetOrdinal("tdspaid")) ? null : reader.GetString(reader.GetOrdinal("tdspaid")),
                tdsdate = reader.IsDBNull(reader.GetOrdinal("tdsdate")) ? null : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("tdsdate")))
            };
        }

    }
}
