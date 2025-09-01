using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Internal_Portal.Repository
{
    public class CompanyRepository : ICompany
    {
        private readonly ISqlConnectionFactory _factory;
        public CompanyRepository(ISqlConnectionFactory factory) => _factory = factory;

        public async Task<IEnumerable<CompanyMaster>> GetAllAsync()
        {
            var results = new List<CompanyMaster>();
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.CompanyMaster_GetAll";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapCompany(reader));
            }
            return results;
        }

        public async Task<CompanyMaster?> GetByCodeAsync(string companyCode)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.CompanyMaster_GetById";
            cmd.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 20) { Value = companyCode });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapCompany(reader);
            return null;
        }

        public async Task<CompanyMaster> InsertUpdateAsync(CompanyMaster m)
        {
            if (m is null) throw new ArgumentNullException(nameof(m));

            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.CompanyMaster_InsertUpdate";

            // Add params, using DBNull.Value for nulls
            cmd.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 20) { Value = (object?)m.CompanyCode ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CompanyName", SqlDbType.VarChar, 255) { Value = (object?)m.CompanyName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CompanyAddress", SqlDbType.NVarChar, -1) { Value = (object?)m.CompanyAddress ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Gstn", SqlDbType.VarChar, 50) { Value = (object?)m.Gstn ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PanNumber", SqlDbType.VarChar, 20) { Value = (object?)m.PanNumber ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ContactPersonName", SqlDbType.VarChar, 100) { Value = (object?)m.ContactPersonName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ContactPersonNumber", SqlDbType.VarChar, 20) { Value = (object?)m.ContactPersonNumber ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PaymentTerms", SqlDbType.VarChar, 100) { Value = (object?)m.PaymentTerms ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@BankName", SqlDbType.VarChar, 100) { Value = (object?)m.BankName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AccountName", SqlDbType.VarChar, 100) { Value = (object?)m.AccountName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@BankAccountNumber", SqlDbType.VarChar, 50) { Value = (object?)m.BankAccountNumber ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IfscCode", SqlDbType.VarChar, 20) { Value = (object?)m.IfscCode ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Micr", SqlDbType.VarChar, 20) { Value = (object?)m.Micr ?? DBNull.Value });

            // Merge SP SELECTs the saved row at the end, so read it
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapCompany(reader);

            // If no result returned, throw or return empty. We'll throw to indicate unexpected behavior.
            throw new InvalidOperationException("Merge did not return the saved CompanyMaster row.");
        }

        public async Task<bool> DeleteAsync(string companyCode)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.CompanyMaster_Delete";
            cmd.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 20) { Value = companyCode });

            var result = await cmd.ExecuteScalarAsync();
            return result != null && Convert.ToInt32(result) > 0;
        }


        // Simple mapper: names must match column names returned by SP
        private static CompanyMaster MapCompany(SqlDataReader reader)
        {
            return new CompanyMaster
            {
                CompanyCode = reader["company_code"] as string,
                CompanyName = reader["company_name"] as string,
                CompanyAddress = reader["company_address"] as string,
                Gstn = reader["gstn"] as string,
                PanNumber = reader["pan_number"] as string,
                ContactPersonName = reader["contact_person_name"] as string,
                ContactPersonNumber = reader["contact_person_number"] as string,
                PaymentTerms = reader["payment_terms"] as string,
                BankName = reader["bank_name"] as string,
                AccountName = reader["account_name"] as string,
                BankAccountNumber = reader["bank_account_number"] as string,
                IfscCode = reader["ifsc_code"] as string,
                Micr = reader["micr"] as string
            };
        }
    }
}