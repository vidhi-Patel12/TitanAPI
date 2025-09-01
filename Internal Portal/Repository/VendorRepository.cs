using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class VendorRepository : IVendor
    {
        private readonly ISqlConnectionFactory _factory;

        public VendorRepository(ISqlConnectionFactory factory) => _factory = factory;

        public async Task<IEnumerable<VendorMaster>> GetAllAsync()
        {
            var result = new List<VendorMaster>();
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.VendorMaster_GetAll";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(MapVendor(reader));
            }
            return result;
        }

        public async Task<VendorMaster?> GetByIdAsync(int vendorId)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.VendorMaster_GetById";
            cmd.Parameters.Add(new SqlParameter("@VendorId", SqlDbType.Int) { Value = vendorId });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapVendor(reader);
            }
            return null;
        }

        public async Task<VendorMaster> InsertUpdateAsync(VendorMaster vendor)
        {
            if (vendor == null) throw new ArgumentNullException(nameof(vendor));

            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.VendorMaster_InsertUpdate";

            cmd.Parameters.Add(new SqlParameter("@VendorId", SqlDbType.Int) { Value = (object?)vendor.VendorId ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 20) { Value = (object?)vendor.CompanyCode ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@VendorName", SqlDbType.NVarChar, 255) { Value = (object?)vendor.VendorName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, -1) { Value = (object?)vendor.Address ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Gstn", SqlDbType.VarChar, 50) { Value = (object?)vendor.Gstn ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PanNumber", SqlDbType.VarChar, 20) { Value = (object?)vendor.PanNumber ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ContactPersonName", SqlDbType.VarChar, 100) { Value = (object?)vendor.ContactPersonName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ContactPersonNumber", SqlDbType.VarChar, 20) { Value = (object?)vendor.ContactPersonNumber ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PaymentTerms", SqlDbType.VarChar, 100) { Value = (object?)vendor.PaymentTerms ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@GstnUpload", SqlDbType.VarChar, 255) { Value = (object?)vendor.GstnUpload ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PanUpload", SqlDbType.VarChar, 255) { Value = (object?)vendor.PanUpload ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AccountHolderName", SqlDbType.VarChar, 100) { Value = (object?)vendor.AccountHolderName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AccountNumber1", SqlDbType.VarChar, 50) { Value = (object?)vendor.AccountNumber1 ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IfscCode", SqlDbType.VarChar, 20) { Value = (object?)vendor.IfscCode ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@BankAccountName", SqlDbType.VarChar, 100) { Value = (object?)vendor.BankAccountName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CancelledCheque", SqlDbType.VarChar, 255) { Value = (object?)vendor.CancelledCheque ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Agreement1", SqlDbType.VarChar, 255) { Value = (object?)vendor.Agreement1 ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Agreement2", SqlDbType.VarChar, 255) { Value = (object?)vendor.Agreement2 ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Agreement3", SqlDbType.VarChar, 255) { Value = (object?)vendor.Agreement3 ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Agreement4", SqlDbType.VarChar, 255) { Value = (object?)vendor.Agreement4 ?? DBNull.Value });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapVendor(reader);

            throw new InvalidOperationException("Merge did not return the saved VendorMaster row.");
        }

        public async Task<bool> DeleteAsync(int vendorId)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.VendorMaster_Delete";
            cmd.Parameters.Add(new SqlParameter("@VendorId", SqlDbType.Int) { Value = vendorId });

            var result = await cmd.ExecuteScalarAsync();
            return result != null && Convert.ToInt32(result) > 0;
        }

        private static VendorMaster MapVendor(SqlDataReader reader)
        {
            return new VendorMaster
            {
                VendorId = Convert.ToInt32(reader["vendor_id"]),
                CompanyCode = reader["company_code"] as string,
                VendorName = reader["vendor_name"] as string,
                Address = reader["address"] as string,
                Gstn = reader["gstn"] as string,
                PanNumber = reader["pan_number"] as string,
                ContactPersonName = reader["contact_person_name"] as string,
                ContactPersonNumber = reader["contact_person_number"] as string,
                PaymentTerms = reader["payment_terms"] as string,
                GstnUpload = reader["gstn_upload"] as string,
                PanUpload = reader["pan_upload"] as string,
                AccountHolderName = reader["account_holder_name"] as string,
                AccountNumber1 = reader["account_number1"] as string,
                IfscCode = reader["ifsc_code"] as string,
                BankAccountName = reader["bank_account_name"] as string,
                CancelledCheque = reader["cancelled_cheque"] as string,
                Agreement1 = reader["agreement1"] as string,
                Agreement2 = reader["agreement2"] as string,
                Agreement3 = reader["agreement3"] as string,
                Agreement4 = reader["agreement4"] as string
            };
        }
    }
}