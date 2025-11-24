using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class UserRolePermissionRepository : IUserRolePermission
    {
        private readonly string _connectionString;

        public UserRolePermissionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //public async Task<int> InsertOrUpdateAsync(RolePermissionMaster rolePermission)
        //{
        //    using var conn = new SqlConnection(_connectionString);
        //    using var cmd = new SqlCommand("InsertUpdateRolePermission", conn)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };

        //    cmd.Parameters.AddWithValue("@Id", rolePermission.Id == 0 ? (object)DBNull.Value : rolePermission.Id);
        //    cmd.Parameters.AddWithValue("@RoleId", rolePermission.RoleId);
        //    cmd.Parameters.AddWithValue("@PermissionId", rolePermission.PermissionId);

        //    await conn.OpenAsync();
        //    var result = await cmd.ExecuteScalarAsync();
        //    return Convert.ToInt32(result);
        //}


        public async Task<int> InsertOrUpdateAsync(int roleId, int[] permissionIds, int? updateId = null)
        {
            if (permissionIds == null || permissionIds.Length == 0) return 0;

            int count = 0;

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            foreach (var permId in permissionIds)
            {
                using var cmd = new SqlCommand("InsertUpdateRolePermission", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // If updating a single row, pass updateId for the first permission
                int? idParam = updateId.HasValue ? updateId.Value : (int?)null;

                cmd.Parameters.AddWithValue("@Id", idParam.HasValue ? (object)idParam.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@RoleId", roleId);
                cmd.Parameters.AddWithValue("@PermissionId", permId);

                var result = await cmd.ExecuteScalarAsync();
                count += Convert.ToInt32(result);
            }

            return count;
        }


        //public async Task<bool> DeleteAsync(int id)
        //{
        //    using var conn = new SqlConnection(_connectionString);
        //    using var cmd = new SqlCommand("DeleteRolePermission", conn)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };
        //    cmd.Parameters.AddWithValue("@Id", id);

        //    await conn.OpenAsync();
        //    var rows = await cmd.ExecuteNonQueryAsync();
        //    return rows > 0;
        //}


        public async Task<bool> DeleteByRoleIdAsync(int roleId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("[DeleteRolePermission]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@RoleId", roleId);

            await conn.OpenAsync();

            // ExecuteNonQuery returns number of rows deleted
            var rows = await cmd.ExecuteNonQueryAsync();

            return rows > 0;
        }


        public async Task<IEnumerable<RolePermissionMaster>> GetAllAsync()
        {
            var list = new List<RolePermissionMaster>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("GetAllRolePermissions", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new RolePermissionMaster
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                    PermissionId = reader.GetInt32(reader.GetOrdinal("PermissionId")),
                    Role = new UserRoleMaster
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("RoleId")),
                        RoleName = reader["RoleName"].ToString()
                    },
                    Permission = new PermissionMaster
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("PermissionId")),
                        PermissionName = reader["PermissionName"].ToString()
                    }
                });
            }

            return list;
        }

        public async Task<IEnumerable<PermissionMaster>> GetAllPermissionsAsync()
        {
            var roles = new List<PermissionMaster>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("GetAllPermissions", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        roles.Add(new PermissionMaster
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PermissionName = reader["PermissionName"].ToString()!
                        });
                    }
                }
            }

            return roles;
        }


        public async Task<RolePermissionMaster?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("GetRolePermissionById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new RolePermissionMaster
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                    PermissionId = reader.GetInt32(reader.GetOrdinal("PermissionId")),
                    Role = new UserRoleMaster
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("RoleId")),
                        RoleName = reader["RoleName"].ToString()
                    },
                    Permission = new PermissionMaster
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("PermissionId")),
                        PermissionName = reader["PermissionName"].ToString()
                    }
                };
            }

            return null;
        }
    }
}