using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class JobApplicationRepository : IJobApplication
    {
        private readonly string _connectionString;

        public JobApplicationRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<int> InsertApplicationAsync(JobApplication application)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("InsertJobApplication", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CareerId", application.CareerId);
                cmd.Parameters.AddWithValue("@ApplicantName", application.ApplicantName);
                cmd.Parameters.AddWithValue("@Email", application.Email);
                cmd.Parameters.AddWithValue("@Phone", (object?)application.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ResumeUrl", (object?)application.ResumeUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CoverLetter", (object?)application.CoverLetter ?? DBNull.Value);

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }

        public async Task<string> GetJobTitleByIdAsync(int careerId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT JobTitle FROM Career WHERE CareerId = @CareerId", conn);
            cmd.Parameters.AddWithValue("@CareerId", careerId);
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() ?? string.Empty;
        }

    }
}