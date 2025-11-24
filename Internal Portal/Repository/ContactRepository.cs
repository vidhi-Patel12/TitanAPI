using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class ContactRepository : IContact
    {
        private readonly string _connectionString;

        public ContactRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<int> InsertMessage(ContactMessage message)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("InsertContactMessage", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name", message.Name);
                cmd.Parameters.AddWithValue("@CompanyName", message.CompanyName);
                cmd.Parameters.AddWithValue("@Email", message.Email);
                cmd.Parameters.AddWithValue("@Phone", message.Phone);
                cmd.Parameters.AddWithValue("@Message", message.Message);

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}