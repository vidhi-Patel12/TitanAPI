using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class UserRoleRepository : IUserRole
    {
        private readonly string _connectionString;

        public UserRoleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<UserRoleMaster>> GetAllAsync()
        {
            var roles = new List<UserRoleMaster>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("GetAllUserRoles", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        roles.Add(new UserRoleMaster
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            RoleName = reader["role_name"].ToString()!
                        });
                    }
                }
            }

            return roles;
        }

        public async Task<UserRoleMaster?> GetByIdAsync(int id)
        {
            UserRoleMaster? role = null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("GetUserRoleById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        role = new UserRoleMaster
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            RoleName = reader["role_name"].ToString()!
                        };
                    }
                }
            }

            return role;
        }

        public async Task<int> InsertOrUpdateAsync(UserRoleMaster role)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("InsertUpdateUserRole", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", role.Id == 0 ? (object)DBNull.Value : role.Id);
                cmd.Parameters.AddWithValue("@RoleName", role.RoleName);

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("DeleteUserRole", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();
                var rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
        }
    }
}