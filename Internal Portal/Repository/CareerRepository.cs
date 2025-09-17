using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class CareerRepository : ICareer
    {
        private readonly string _connectionString;

        public CareerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateCareer(Career career)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("CreateCareer", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Employementtype", career.Employementtype);
                cmd.Parameters.AddWithValue("@Location", career.Location);
                cmd.Parameters.AddWithValue("@JobTitle", career.JobTitle);
                cmd.Parameters.AddWithValue("@JobDescription", (object?)career.JobDescription ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", career.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", career.CreatedBy);

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }

        public async Task<Career> GetCareerById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("GetCareerById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CareerId", id);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Career
                        {
                            CareerId = reader.GetInt32(reader.GetOrdinal("CareerId")),
                            Employementtype = reader["Employementtype"].ToString(),
                            Location = reader["Location"].ToString(),
                            JobTitle = reader["JobTitle"]?.ToString(),
                            JobDescription = reader["JobDescription"]?.ToString(),     
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

        public async Task<Career?> GetCareerByName(string jobTitle)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("GetCareerByName", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@JobTitle", jobTitle);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Career
                        {
                            CareerId = reader.GetInt32(reader.GetOrdinal("CareerId")),
                            Employementtype = reader["Employementtype"].ToString(),
                            Location = reader["Location"]?.ToString(),
                            JobTitle = reader["JobTitle"]?.ToString(),
                            JobDescription = reader["JobDescription"].ToString(),
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

        public async Task<IEnumerable<Career>> GetAllCareers()
        {
            var careers = new List<Career>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("GetAllCareer", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        careers.Add(new Career
                        {
                            CareerId = reader.GetInt32(reader.GetOrdinal("CareerId")),
                            Employementtype = reader["Employementtype"].ToString(),
                            Location = reader["Location"]?.ToString(),
                            JobTitle = reader["JobTitle"]?.ToString(),
                            JobDescription = reader["JobDescription"].ToString(),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : (int?)reader["UpdatedBy"],
                            UpdatedDate = reader["UpdatedDate"] == DBNull.Value ? null : (DateTime?)reader["UpdatedDate"]
                        });
                    }
                }
            }

            return careers;
        }

        public async Task UpdateCareer(Career career)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("UpdateCareer", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CareerId", career.CareerId);
                cmd.Parameters.AddWithValue("@Employementtype", career.Employementtype);
                cmd.Parameters.AddWithValue("@Location", career.Location);
                cmd.Parameters.AddWithValue("@JobTitle", career.JobTitle);
                cmd.Parameters.AddWithValue("@JobDescription", (object?)career.JobDescription ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", career.IsActive);
                cmd.Parameters.AddWithValue("@UpdatedBy", career.UpdatedBy ?? (object)DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> DeleteAsync(int id, int updatedBy)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("DeleteCareer", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@CareerId", SqlDbType.Int) { Value = id });
                cmd.Parameters.Add(new SqlParameter("@UpdatedBy", SqlDbType.Int) { Value = updatedBy });

                await conn.OpenAsync();

                var result = await cmd.ExecuteScalarAsync();
                return result != null && Convert.ToInt32(result) > 0;
            }
        }
    }
}