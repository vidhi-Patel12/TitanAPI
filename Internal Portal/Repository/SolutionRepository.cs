using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class SolutionRepository : ISolution
    {
        private readonly string _connectionString;

        public SolutionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateSolution(Solution solution)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("CreateSolution", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SolutionName", solution.SolutionName);
                cmd.Parameters.AddWithValue("@Description", (object?)solution.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Image", (object?)solution.Image ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", solution.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", solution.CreatedBy);

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }

        public async Task<Solution> GetSolutionById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("GetSolutionById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SolutionId", id);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Solution
                        {
                            SolutionId = reader.GetInt32(reader.GetOrdinal("SolutionId")),
                            SolutionName = reader["SolutionName"].ToString(),
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

        public async Task<Solution?> GetSolutionByName(string solutionName)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("GetSolutionByName", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SolutionName", solutionName);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Solution
                        {
                            SolutionId = reader.GetInt32(reader.GetOrdinal("SolutionId")),
                            SolutionName = reader["SolutionName"].ToString(),
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

        public async Task<IEnumerable<Solution>> GetAllSolutions()
        {
            var solutions = new List<Solution>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("GetAllSolutions", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        solutions.Add(new Solution
                        {
                            SolutionId = reader.GetInt32(reader.GetOrdinal("SolutionId")),
                            SolutionName = reader["SolutionName"].ToString(),
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

            return solutions;
        }

        public async Task UpdateSolution(Solution solution)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("UpdateSolution", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SolutionId", solution.SolutionId);
                cmd.Parameters.AddWithValue("@SolutionName", solution.SolutionName);
                cmd.Parameters.AddWithValue("@Description", (object?)solution.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Image", (object?)solution.Image ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", solution.IsActive);
                cmd.Parameters.AddWithValue("@UpdatedBy", solution.UpdatedBy ?? (object)DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> DeleteAsync(int id, int updatedBy)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("DeleteSolution", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@SolutionId", SqlDbType.Int) { Value = id });
                cmd.Parameters.Add(new SqlParameter("@UpdatedBy", SqlDbType.Int) { Value = updatedBy });

                await conn.OpenAsync();

                var result = await cmd.ExecuteScalarAsync();
                return result != null && Convert.ToInt32(result) > 0;
            }
        }
    }
}
