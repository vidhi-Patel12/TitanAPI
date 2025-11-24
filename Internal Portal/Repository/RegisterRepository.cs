using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Internal_Portal.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Internal_Portal.Repository
{
    public class RegisterRepository : IRegister
    {
        private readonly ISqlConnectionFactory _factory;
        public RegisterRepository(ISqlConnectionFactory factory) => _factory = factory;

        public async Task<IEnumerable<Register>> GetAllAsync()
        {
            var results = new List<Register>();
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.Register_GetAll";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapRegister(reader));
            }
            return results;
        }

        public async Task<Register?> GetByIdAsync(int id)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.Register_GetById";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapRegister(reader);
            return null;
        }

        public async Task<int> InsertAsync(Register m)
        {
            if (m is null) throw new ArgumentNullException(nameof(m));

            await using var conn = _factory.CreateConnection();

            await using var cmd = conn.CreateCommand();

            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.Register_Insert";

                cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.VarChar, 100) { Value = (object?)m.FirstName ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.VarChar, 100) { Value = (object?)m.LastName ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar, 150) { Value = (object?)m.Email ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar, 255) { Value = (object?)m.Password ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@ContactNumber", SqlDbType.VarChar, 20) { Value = (object?)m.contact_number ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@UserRoleId", SqlDbType.Int) { Value = m.UserRoleId });

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SQL ERROR in InsertAsync: " + ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Register m)
        {
            if (m is null) throw new ArgumentNullException(nameof(m));

            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.Register_Update";

            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = m.Id });
            cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.VarChar, 100) { Value = (object?)m.FirstName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.VarChar, 100) { Value = (object?)m.LastName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar, 150) { Value = (object?)m.Email ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar, 255) { Value = (object?)m.Password ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ContactNumber", SqlDbType.VarChar, 20) { Value = (object?)m.contact_number ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@UserRoleId", SqlDbType.Int) { Value = m.UserRoleId });

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0; // success if at least one row updated
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.Register_Delete";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

            var result = await cmd.ExecuteScalarAsync(); // now returns affected rows
            return Convert.ToInt32(result) > 0;
        }

        // Mapper: names must match SP column aliases
        private static Register MapRegister(SqlDataReader reader)
        {
            return new Register
            {
                Id = Convert.ToInt32(reader["id"]),
                FirstName = reader["first_name"] as string,
                LastName = reader["last_name"] as string,
                Email = reader["email"] as string,
                Password = reader["password"] as string,
                contact_number = reader["contact_number"] as string,
                UserRoleId = Convert.ToInt32(reader["user_role_id"]),
                UserRole = new UserRoleMaster
                {
                    Id = Convert.ToInt32(reader["user_role_id"]),
                    RoleName = reader["role_name"] as string
                }
            };
        }
    }
}