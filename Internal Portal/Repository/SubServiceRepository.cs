using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class SubServiceRepository : ISubService
    {
        private readonly string _connectionString;

        public SubServiceRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateSubService(SubService subService)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("CreateSubService", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ServiceId", subService.ServiceId);
                cmd.Parameters.AddWithValue("@SubServiceName", subService.SubServiceName);
                cmd.Parameters.AddWithValue("@Description", (object?)subService.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Image", (object?)subService.Image ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", subService.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", subService.CreatedBy);

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }

        public async Task UpdateSubService(SubService subService)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("UpdateSubService", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SubServiceId", subService.SubServiceId);
                cmd.Parameters.AddWithValue("@ServiceId", subService.ServiceId);
                cmd.Parameters.AddWithValue("@SubServiceName", subService.SubServiceName);
                cmd.Parameters.AddWithValue("@Description", (object?)subService.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Image", (object?)subService.Image ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", subService.IsActive);
                cmd.Parameters.AddWithValue("@UpdatedBy", subService.UpdatedBy ?? (object)DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteSubService(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("DeleteSubService", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SubServiceId", id);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<SubService?> GetSubServiceById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("GetSubServiceById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SubServiceId", id);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new SubService
                        {
                            SubServiceId = reader.GetInt32(reader.GetOrdinal("SubServiceId")),
                            ServiceId = reader.GetInt32(reader.GetOrdinal("ServiceId")),
                            SubServiceName = reader["SubServiceName"].ToString(),
                            Description = reader["Description"]?.ToString(),
                            Image = reader["Image"]?.ToString(),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : (int?)reader["UpdatedBy"],
                            UpdatedDate = reader["UpdatedDate"] == DBNull.Value ? null : (DateTime?)reader["UpdatedDate"]
                        };
                    }
                }
            }
            return null;
        }

        public async Task<SubService?> GetSubServiceByName(string subServiceName)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("GetSubServiceByName", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SubServiceName", subServiceName);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new SubService
                        {
                            SubServiceId = reader.GetInt32(reader.GetOrdinal("SubServiceId")),
                            ServiceId = reader.GetInt32(reader.GetOrdinal("ServiceId")),
                            SubServiceName = reader["SubServiceName"].ToString(),
                            Description = reader["Description"]?.ToString(),
                            Image = reader["Image"]?.ToString(),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : (int?)reader["UpdatedBy"],
                            UpdatedDate = reader["UpdatedDate"] == DBNull.Value ? null : (DateTime?)reader["UpdatedDate"]
                        };
                    }
                }
            }
            return null;
        }

        public async Task<IEnumerable<SubService>> GetAllSubServices()
        {
            var subServices = new List<SubService>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("GetAllSubServices", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        subServices.Add(new SubService
                        {
                            SubServiceId = reader.GetInt32(reader.GetOrdinal("SubServiceId")),
                            ServiceId = reader.GetInt32(reader.GetOrdinal("ServiceId")),
                            SubServiceName = reader["SubServiceName"].ToString(),
                            Description = reader["Description"]?.ToString(),
                            Image = reader["Image"]?.ToString(),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : (int?)reader["UpdatedBy"],
                            UpdatedDate = reader["UpdatedDate"] == DBNull.Value ? null : (DateTime?)reader["UpdatedDate"]
                        });
                    }
                }
            }

            return subServices;
        }
    }
}