using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class ProjectMasterRepository : IProjectMaster
    {
        private readonly ISqlConnectionFactory _factory;
        public ProjectMasterRepository(ISqlConnectionFactory factory) => _factory = factory;

        public async Task<IEnumerable<ProjectMaster>> GetAllAsync()
        {
            var results = new List<ProjectMaster>();
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.ProjectMaster_GetAll";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapProject(reader));
            }
            return results;
        }

        public async Task<ProjectMaster?> GetByCodeAsync(string projectCode)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.ProjectMaster_GetByCode";
            cmd.Parameters.Add(new SqlParameter("@ProjectCode", SqlDbType.VarChar, 20) { Value = projectCode });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapProject(reader);
            return null;
        }

        public async Task<ProjectMaster> InsertUpdateAsync(ProjectMaster project)
        {
            if (project is null) throw new ArgumentNullException(nameof(project));

            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.ProjectMaster_InsertUpdate";

            cmd.Parameters.Add(new SqlParameter("@ProjectCode", SqlDbType.VarChar, 20) { Value = (object?)project.ProjectCode ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, -1) { Value = (object?)project.Description ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.Date) { Value = (object?)project.StartDate ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.Date) { Value = (object?)project.EndDate ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CustomerId", SqlDbType.Int) { Value = (object?)project.CustomerId ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CustomerRate", SqlDbType.Decimal) { Value = (object?)project.CustomerRate ?? DBNull.Value, Precision = 12, Scale = 2 });
            cmd.Parameters.Add(new SqlParameter("@RateUnit", SqlDbType.VarChar, 50) { Value = (object?)project.RateUnit ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@HsCode", SqlDbType.VarChar, 50) { Value = (object?)project.HsCode ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Notes", SqlDbType.NVarChar, -1) { Value = (object?)project.Notes ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ApprovalLevel", SqlDbType.VarChar, 10) { Value = (object?)project.ApprovalLevel ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 10) { Value = (object?)project.Status ?? DBNull.Value });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapProject(reader);

            throw new InvalidOperationException("Merge did not return the saved ProjectMaster row.");
        }

        public async Task<bool> DeleteAsync(string projectCode)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.ProjectMaster_Delete";

            cmd.Parameters.Add(new SqlParameter("@ProjectCode", SqlDbType.VarChar, 20) { Value = projectCode });

            var result = await cmd.ExecuteScalarAsync();
            return result != null && Convert.ToInt32(result) > 0; // true if row deleted
        }


        private static ProjectMaster MapProject(SqlDataReader reader)
        {
            return new ProjectMaster
            {
                ProjectCode = reader["project_code"] as string,
                Description = reader["description"] as string,
                StartDate = reader["start_date"] as DateTime?,
                EndDate = reader["end_date"] as DateTime?,
                CustomerId = reader["customer_id"] as int?,
                CustomerRate = reader["customer_rate"] as decimal?,
                RateUnit = reader["rate_unit"] as string,
                HsCode = reader["hs_code"] as string,
                Notes = reader["notes"] as string,
                ApprovalLevel = reader["approval_level"] as string,
                Status = reader["status"] as string
            };
        }
    }
}