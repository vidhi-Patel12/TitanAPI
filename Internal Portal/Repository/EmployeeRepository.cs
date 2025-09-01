using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class EmployeeRepository : IEmployee
    {
        private readonly ISqlConnectionFactory _factory;
        public EmployeeRepository(ISqlConnectionFactory factory) => _factory = factory;

        public async Task<IEnumerable<EmployeeMaster>> GetAllAsync()
        {
            var list = new List<EmployeeMaster>();
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.EmployeeMaster_GetAll";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapEmployee(reader));
            }
            return list;
        }

        public async Task<EmployeeMaster?> GetByIdAsync(int employeeId)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.EmployeeMaster_GetById";
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = employeeId });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapEmployee(reader);
            return null;
        }

        public async Task<EmployeeMaster> InsertUpdateAsync(EmployeeMaster m)
        {
            if (m is null) throw new ArgumentNullException(nameof(m));

            await using var conn = _factory.CreateConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.EmployeeMaster_InsertUpdate";

            cmd.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = m.EmployeeId <= 0 ? (object)DBNull.Value : m.EmployeeId });
            cmd.Parameters.Add(new SqlParameter("@EmployeeType", SqlDbType.NVarChar, 20) { Value = m.EmployeeType });
            cmd.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.NVarChar, 20) { Value = m.CompanyCode });
            cmd.Parameters.Add(new SqlParameter("@VendorId", SqlDbType.Int) { Value = m.VendorId ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 255) { Value = m.Name });
            cmd.Parameters.Add(new SqlParameter("@AltName", SqlDbType.NVarChar, 255) { Value = m.AltName ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Age", SqlDbType.Int) { Value = m.Age ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@SkillSet", SqlDbType.NVarChar, 255) { Value = m.SkillSet ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Experience", SqlDbType.Decimal) { Value = m.Experience ?? (object)DBNull.Value, Precision = 5, Scale = 2 });
            cmd.Parameters.Add(new SqlParameter("@TimingAvailability", SqlDbType.NVarChar, 20) { Value = m.TimingAvailability ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ContactNumber1", SqlDbType.NVarChar, 20) { Value = m.ContactNumber1 ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ContactNumber2", SqlDbType.NVarChar, 20) { Value = m.ContactNumber2 ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Remarks", SqlDbType.NVarChar, -1) { Value = m.Remarks ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ReferredBy", SqlDbType.NVarChar, 100) { Value = m.ReferredBy ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = m.CreatedBy ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@NdaUpload", SqlDbType.NVarChar, 255) { Value = m.NdaUpload ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AadharUpload", SqlDbType.NVarChar, 255) { Value = m.AadharUpload ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PanNumber", SqlDbType.NVarChar, 20) { Value = m.PanNumber ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PanUpload", SqlDbType.NVarChar, 255) { Value = m.PanUpload ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AccountNumber1", SqlDbType.NVarChar, 50) { Value = m.AccountNumber1 ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IfscCode1", SqlDbType.NVarChar, 20) { Value = m.IfscCode1 ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AccountName1", SqlDbType.NVarChar, 100) { Value = m.AccountName1 ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Cheque1Upload", SqlDbType.NVarChar, 255) { Value = m.Cheque1Upload ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Aadhar2Upload", SqlDbType.NVarChar, 255) { Value = m.Aadhar2Upload ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PanNumber2", SqlDbType.NVarChar, 20) { Value = m.PanNumber2 ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PanUpload2", SqlDbType.NVarChar, 255) { Value = m.PanUpload2 ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AccountNumber2", SqlDbType.NVarChar, 50) { Value = m.AccountNumber2 ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IfscCode2", SqlDbType.NVarChar, 20) { Value = m.IfscCode2 ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AccountName2", SqlDbType.NVarChar, 100) { Value = m.AccountName2 ?? (object)DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Cheque2Upload", SqlDbType.NVarChar, 255) { Value = m.Cheque2Upload ?? (object)DBNull.Value });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new EmployeeMaster
                {
                    EmployeeId = reader.GetInt32(reader.GetOrdinal("employee_id")),
                    EmployeeType = reader.GetString(reader.GetOrdinal("employee_type")),
                    CompanyCode = reader.GetString(reader.GetOrdinal("company_code")),
                    VendorId = reader.IsDBNull(reader.GetOrdinal("vendor_id")) ? null : reader.GetInt32(reader.GetOrdinal("vendor_id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    AltName = reader.IsDBNull(reader.GetOrdinal("alt_name")) ? null : reader.GetString(reader.GetOrdinal("alt_name")),
                    Age = reader.IsDBNull(reader.GetOrdinal("age")) ? null : reader.GetInt32(reader.GetOrdinal("age")),
                    SkillSet = reader.IsDBNull(reader.GetOrdinal("skill_set")) ? null : reader.GetString(reader.GetOrdinal("skill_set")),
                    Experience = reader.IsDBNull(reader.GetOrdinal("experience")) ? null : reader.GetDecimal(reader.GetOrdinal("experience")),
                    TimingAvailability = reader.IsDBNull(reader.GetOrdinal("timing_availability")) ? null : reader.GetString(reader.GetOrdinal("timing_availability")),
                    ContactNumber1 = reader.IsDBNull(reader.GetOrdinal("contact_number1")) ? null : reader.GetString(reader.GetOrdinal("contact_number1")),
                    ContactNumber2 = reader.IsDBNull(reader.GetOrdinal("contact_number2")) ? null : reader.GetString(reader.GetOrdinal("contact_number2")),
                    Remarks = reader.IsDBNull(reader.GetOrdinal("remarks")) ? null : reader.GetString(reader.GetOrdinal("remarks")),
                    ReferredBy = reader.IsDBNull(reader.GetOrdinal("referred_by")) ? null : reader.GetString(reader.GetOrdinal("referred_by")),
                    CreatedBy = reader.IsDBNull(reader.GetOrdinal("created_by")) ? null : reader.GetString(reader.GetOrdinal("created_by")),
                    NdaUpload = reader.IsDBNull(reader.GetOrdinal("nda_upload")) ? null : reader.GetString(reader.GetOrdinal("nda_upload")),
                    AadharUpload = reader.IsDBNull(reader.GetOrdinal("aadhar_upload")) ? null : reader.GetString(reader.GetOrdinal("aadhar_upload")),
                    PanNumber = reader.IsDBNull(reader.GetOrdinal("pan_number")) ? null : reader.GetString(reader.GetOrdinal("pan_number")),
                    PanUpload = reader.IsDBNull(reader.GetOrdinal("pan_upload")) ? null : reader.GetString(reader.GetOrdinal("pan_upload")),
                    AccountNumber1 = reader.IsDBNull(reader.GetOrdinal("account_number1")) ? null : reader.GetString(reader.GetOrdinal("account_number1")),
                    IfscCode1 = reader.IsDBNull(reader.GetOrdinal("ifsc_code1")) ? null : reader.GetString(reader.GetOrdinal("ifsc_code1")),
                    AccountName1 = reader.IsDBNull(reader.GetOrdinal("account_name1")) ? null : reader.GetString(reader.GetOrdinal("account_name1")),
                    Cheque1Upload = reader.IsDBNull(reader.GetOrdinal("cheque1_upload")) ? null : reader.GetString(reader.GetOrdinal("cheque1_upload")),
                    Aadhar2Upload = reader.IsDBNull(reader.GetOrdinal("aadhar2_upload")) ? null : reader.GetString(reader.GetOrdinal("aadhar2_upload")),
                    PanNumber2 = reader.IsDBNull(reader.GetOrdinal("pan_number2")) ? null : reader.GetString(reader.GetOrdinal("pan_number2")),
                    PanUpload2 = reader.IsDBNull(reader.GetOrdinal("pan_upload2")) ? null : reader.GetString(reader.GetOrdinal("pan_upload2")),
                    AccountNumber2 = reader.IsDBNull(reader.GetOrdinal("account_number2")) ? null : reader.GetString(reader.GetOrdinal("account_number2")),
                    IfscCode2 = reader.IsDBNull(reader.GetOrdinal("ifsc_code2")) ? null : reader.GetString(reader.GetOrdinal("ifsc_code2")),
                    AccountName2 = reader.IsDBNull(reader.GetOrdinal("account_name2")) ? null : reader.GetString(reader.GetOrdinal("account_name2")),
                    Cheque2Upload = reader.IsDBNull(reader.GetOrdinal("cheque2_upload")) ? null : reader.GetString(reader.GetOrdinal("cheque2_upload")),
                };
            }

            return null;
        }

        public async Task<bool> DeleteAsync(int employeeId)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.EmployeeMaster_Delete";
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = employeeId });

            var result = await cmd.ExecuteScalarAsync();
            return result != null && Convert.ToInt32(result) > 0;
        }

        private static EmployeeMaster MapEmployee(SqlDataReader r)
        {
            return new EmployeeMaster
            {
                EmployeeId = r["employee_id"] == DBNull.Value ? 0 : Convert.ToInt32(r["employee_id"]),
                EmployeeType = r["employee_type"] as string,
                CompanyCode = r["company_code"] as string,
                VendorId = r["vendor_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["vendor_id"]),
                Name = r["name"] as string,
                AltName = r["alt_name"] as string,
                Age = r["age"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["age"]),
                SkillSet = r["skill_set"] as string,
                Experience = r["experience"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["experience"]),
                TimingAvailability = r["timing_availability"] as string,
                ContactNumber1 = r["contact_number1"] as string,
                ContactNumber2 = r["contact_number2"] as string,
                Remarks = r["remarks"] as string,
                ReferredBy = r["referred_by"] as string,
                CreatedBy = r["created_by"] as string,
                NdaUpload = r["nda_upload"] as string,
                AadharUpload = r["aadhar_upload"] as string,
                PanNumber = r["pan_number"] as string,
                PanUpload = r["pan_upload"] as string,
                AccountNumber1 = r["account_number1"] as string,
                IfscCode1 = r["ifsc_code1"] as string,
                AccountName1 = r["account_name1"] as string,
                Cheque1Upload = r["cheque1_upload"] as string,
                Aadhar2Upload = r["aadhar2_upload"] as string,
                PanNumber2 = r["pan_number2"] as string,
                PanUpload2 = r["pan_upload2"] as string,
                AccountNumber2 = r["account_number2"] as string,
                IfscCode2 = r["ifsc_code2"] as string,
                AccountName2 = r["account_name2"] as string,
                Cheque2Upload = r["cheque2_upload"] as string
            };
        }
    }
}
