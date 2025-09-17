using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class ServiceRepository : IService
    {
        private readonly string _connectionString;

        public ServiceRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateService(Service service)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("CreateService", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                cmd.Parameters.AddWithValue("@Description", (object?)service.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Image", (object?)service.Image ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", service.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", service.CreatedBy);

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }

        public async Task<Service> GetServiceById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("GetServiceById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ServiceId", id);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Service
                        {
                            ServiceId = reader.GetInt32(reader.GetOrdinal("ServiceId")),
                            ServiceName = reader["ServiceName"].ToString(),
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

        public async Task<Service?> GetServiceByName(string serviceName)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("GetServiceByName", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ServiceName", serviceName);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Service
                        {
                            ServiceId = reader.GetInt32(reader.GetOrdinal("ServiceId")),
                            ServiceName = reader["ServiceName"].ToString(),
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

        public async Task<IEnumerable<Service>> GetAllServices()
        {
            var services = new List<Service>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("GetAllServices", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        services.Add(new Service
                        {
                            ServiceId = reader.GetInt32(reader.GetOrdinal("ServiceId")),
                            ServiceName = reader["ServiceName"].ToString(),
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

            return services;
        }

        public async Task UpdateService(Service service)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("UpdateService", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ServiceId", service.ServiceId);
                cmd.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                cmd.Parameters.AddWithValue("@Description", (object?)service.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Image", (object?)service.Image ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", service.IsActive);
                cmd.Parameters.AddWithValue("@UpdatedBy", service.UpdatedBy ?? (object)DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> DeleteAsync(int id, int updatedBy)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("DeleteService", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ServiceId", SqlDbType.Int) { Value = id });
                cmd.Parameters.Add(new SqlParameter("@UpdatedBy", SqlDbType.Int) { Value = updatedBy });

                await conn.OpenAsync();

                var result = await cmd.ExecuteScalarAsync();
                return result != null && Convert.ToInt32(result) > 0;
            }
        }
    }
}