using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class ProjectEmployeeRepository : IProjectEmployee
    {
        private readonly ISqlConnectionFactory _factory;

        public ProjectEmployeeRepository(ISqlConnectionFactory factory) => _factory = factory;

        public async Task<IEnumerable<ProjectEmployee>> GetAllAsync()
        {
            var results = new List<ProjectEmployee>();
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.ProjectEmployee_GetAll";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapProjectEmployee(reader));
            }

            return results;
        }

        public async Task<ProjectEmployee?> GetByIdAsync(int id)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.ProjectEmployee_GetById";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapProjectEmployee(reader);
            return null;
        }

        public async Task<ProjectEmployee> InsertUpdateAsync(ProjectEmployee m)
        {
            if (m is null) throw new ArgumentNullException(nameof(m));

            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.ProjectEmployee_InsertUpdate";

            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = (object?)m.Id ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ProjectCode", SqlDbType.VarChar, 20) { Value = (object?)m.ProjectCode ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = m.EmployeeId });
            cmd.Parameters.Add(new SqlParameter("@EmployeeType", SqlDbType.VarChar, 50) { Value = (object?)m.EmployeeType ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Technology", SqlDbType.VarChar, 50) { Value = (object?)m.Technology ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AllocationType", SqlDbType.VarChar, 20) { Value = (object?)m.AllocationType ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CommissionType", SqlDbType.NVarChar, 20) { Value = (object?)m.CommissionType ?? DBNull.Value }); cmd.Parameters.Add(new SqlParameter("@ConsultantRate", SqlDbType.Decimal) { Value = (object?)m.ConsultantRate ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@RateUnit", SqlDbType.VarChar, 50) { Value = (object?)m.RateUnit ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@TimesheetType", SqlDbType.VarChar, 20) { Value = (object?)m.TimesheetType ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@SapModule", SqlDbType.VarChar, 100) { Value = (object?)m.SapModule ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@EmployeeStartDate", SqlDbType.Date) { Value = (object?)m.EmployeeStartDate ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@EmployeeEndDate", SqlDbType.Date) { Value = (object?)m.EmployeeEndDate ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CycleStartDay", SqlDbType.Int) { Value = (object?)m.CycleStartDay ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CycleEndDay", SqlDbType.Int) { Value = (object?)m.CycleEndDay ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PaymentMode", SqlDbType.VarChar, 10) { Value = (object?)m.PaymentMode ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@TimesheetRequired", SqlDbType.Bit) { Value = (object?)m.TimesheetRequired ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CustomerRate", SqlDbType.Decimal) { Value = (object?)m.CustomerRate ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Inactive", SqlDbType.Bit) { Value = (object?)m.Inactive ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@SalaryPaymentDays", SqlDbType.Int) { Value = (object?)m.SalaryPaymentDays ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@TdsPercent", SqlDbType.Decimal) { Value = (object?)m.TdsPercent ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AccountPreference", SqlDbType.VarChar, 20) { Value = (object?)m.AccountPreference ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@TextFields", SqlDbType.NVarChar, -1) { Value = (object?)m.TextFields ?? DBNull.Value });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapProjectEmployee(reader);

            throw new InvalidOperationException("Merge did not return the saved ProjectEmployee row.");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.ProjectEmployee_Delete"; // You need to create this SP
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

            var result = await cmd.ExecuteScalarAsync();
            return result != null && Convert.ToInt32(result) > 0;
        }

        private static ProjectEmployee MapProjectEmployee(SqlDataReader reader)
        {
            return new ProjectEmployee
            {
                Id = (int)reader["id"],
                ProjectCode = reader["project_code"] as string,
                EmployeeId = (int)reader["employee_id"],
                EmployeeType = reader["employee_type"] as string,
                Technology = reader["technology"] as string,
                AllocationType = reader["allocation_type"] as string,
                CommissionType = reader["commission_type"] as string,
                ConsultantRate = reader["consultant_rate"] as decimal?,
                RateUnit = reader["rate_unit"] as string,
                TimesheetType = reader["timesheet_type"] as string,
                SapModule = reader["sap_module"] as string,
                EmployeeStartDate = reader["employee_start_date"] as DateTime?,
                EmployeeEndDate = reader["employee_end_date"] as DateTime?,
                CycleStartDay = reader["cycle_start_day"] as int?,
                CycleEndDay = reader["cycle_end_day"] as int?,
                PaymentMode = reader["payment_mode"] as string,
                TimesheetRequired = reader["timesheet_required"] as bool?,
                CustomerRate = reader["customer_rate"] as decimal?,
                Inactive = reader["inactive"] as bool?,
                SalaryPaymentDays = reader["salary_payment_days"] as int?,
                TdsPercent = reader["tds_percent"] as decimal?,
                AccountPreference = reader["account_preference"] as string,
                TextFields = reader["text_fields"] as string
            };
        }
    }
}