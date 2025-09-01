using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class CustomerRepository : ICustomer
    {
        private readonly ISqlConnectionFactory _factory;
        public CustomerRepository(ISqlConnectionFactory factory) => _factory = factory;

        public async Task<IEnumerable<CustomerMaster>> GetAllAsync()
        {
            var list = new List<CustomerMaster>();

            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.CustomerMaster_GetAll";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapCustomer(reader));
            }

            return list;
        }

        public async Task<CustomerMaster?> GetByIdAsync(int customerId)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.CustomerMaster_GetById";
            cmd.Parameters.Add(new SqlParameter("@CustomerId", SqlDbType.Int) { Value = customerId });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapCustomer(reader);
            return null;
        }

        public async Task<CustomerMaster> InsertUpdateAsync(CustomerMaster m)
        {
            if (m is null) throw new ArgumentNullException(nameof(m));

            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.CustomerMaster_InsertUpdate";

            // If CustomerId <= 0 treat as NULL for new insert
            var customerIdParamValue = m.CustomerId <= 0 ? (object)DBNull.Value : m.CustomerId;

            cmd.Parameters.Add(new SqlParameter("@CustomerId", SqlDbType.Int) { Value = customerIdParamValue });
            cmd.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 20) { Value = (object?)m.CompanyCode ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.NVarChar, 255) { Value = (object?)m.CustomerName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, -1) { Value = (object?)m.Address ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Country", SqlDbType.NVarChar, 100) { Value = (object?)m.Country ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Gstn", SqlDbType.VarChar, 50) { Value = (object?)m.Gstn ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PanNumber", SqlDbType.VarChar, 20) { Value = (object?)m.PanNumber ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ContactPersonName", SqlDbType.NVarChar, 100) { Value = (object?)m.ContactPersonName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ContactPersonNumber", SqlDbType.VarChar, 20) { Value = (object?)m.ContactPersonNumber ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PaymentTerms", SqlDbType.NVarChar, 100) { Value = (object?)m.PaymentTerms ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Agreement1", SqlDbType.NVarChar, 255) { Value = (object?)m.Agreement1 ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Agreement2", SqlDbType.NVarChar, 255) { Value = (object?)m.Agreement2 ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Agreement3", SqlDbType.NVarChar, 255) { Value = (object?)m.Agreement3 ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Agreement4", SqlDbType.NVarChar, 255) { Value = (object?)m.Agreement4 ?? DBNull.Value });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapCustomer(reader);

            throw new InvalidOperationException("CustomerMaster_InsertUpdate did not return the saved row.");
        }

        public async Task<bool> DeleteAsync(int customerId)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.CustomerMaster_Delete";
            cmd.Parameters.Add(new SqlParameter("@CustomerId", SqlDbType.Int) { Value = customerId });

            // SP returns @@ROWCOUNT in first column — use ExecuteScalarAsync
            var result = await cmd.ExecuteScalarAsync();
            return result != null && Convert.ToInt32(result) > 0;
        }

        private static CustomerMaster MapCustomer(SqlDataReader r)
        {
            return new CustomerMaster
            {
                CustomerId = r["customer_id"] == DBNull.Value ? 0 : Convert.ToInt32(r["customer_id"]),
                CompanyCode = r["company_code"] as string,
                CustomerName = r["customer_name"] as string,
                Address = r["address"] as string,
                Country = r["country"] as string,
                Gstn = r["gstn"] as string,
                PanNumber = r["pan_number"] as string,
                ContactPersonName = r["contact_person_name"] as string,
                ContactPersonNumber = r["contact_person_number"] as string,
                PaymentTerms = r["payment_terms"] as string,
                Agreement1 = r["agreement1"] as string,
                Agreement2 = r["agreement2"] as string,
                Agreement3 = r["agreement3"] as string,
                Agreement4 = r["agreement4"] as string
            };
        }
    }
}