using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Internal_Portal.Repository
{
    public class DropdownMasterRepository : IDropdownMaster
    {
        private readonly ISqlConnectionFactory _factory;

        public DropdownMasterRepository(ISqlConnectionFactory factory) => _factory = factory;

        public async Task<IEnumerable<DropdownMaster>> GetAllAsync()
        {
            var results = new List<DropdownMaster>();
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.GetAllDropdownMaster";

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapDropdown(reader));
            }
            return results;
        }

        public async Task<DropdownMaster?> GetByIdAsync(int id)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.GetDropdownMasterById";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapDropdown(reader);
            return null;
        }


        public async Task<IEnumerable<DropdownMaster>> GetByNameAsync(string name)
        {
            var list = new List<DropdownMaster>();

            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.GetDropdownMasterByName";
            cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar, 100) { Value = name });

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapDropdown(reader));
            }

            return list;
        }


        public async Task<DropdownMaster> InsertUpdateAsync(DropdownMaster model)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            try
            {
                await using var conn = _factory.CreateConnection();
                await using var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.DropdownMaster_InsertUpdate";

                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = model.Id });
                cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar, 100) { Value = (object?)model.Name ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Value", SqlDbType.NVarChar, -1) { Value = (object?)model.Value ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = model.IsActive });
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = model.CreatedBy });
                cmd.Parameters.AddWithValue("@UpdatedBy", model.UpdatedBy ?? (object)DBNull.Value);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return MapDropdown(reader);
                }

                throw new InvalidOperationException("Insert/Update failed, no data returned.");

            }
            catch (Exception ex)
            {
                // TEMP: log full details — later replace with proper logger
                Console.WriteLine($"❌ DB ERROR: {ex.Message}");
                throw; // rethrow to bubble to middleware
            }
        }

        //public async Task<DropdownMaster> InsertUpdateAsync(DropdownMaster model)
        //{
        //    if (model is null) throw new ArgumentNullException(nameof(model));

        //    await using var conn = _factory.CreateConnection();
        //    await using var cmd = conn.CreateCommand();
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.CommandText = "dbo.DropdownMaster_InsertUpdate";

        //    cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = model.Id });
        //    cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar, 100) { Value = (object?)model.Name ?? DBNull.Value });
        //    cmd.Parameters.Add(new SqlParameter("@Value", SqlDbType.VarChar, -1) { Value = (object?)model.Value ?? DBNull.Value });
        //    cmd.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = model.IsActive });
        //    cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
        //    cmd.Parameters.AddWithValue("@UpdatedBy", model.UpdatedBy ?? (object)DBNull.Value);


        //    // SP returns a single row with Id
        //    var result = await cmd.ExecuteScalarAsync();
        //    if (result == null) throw new InvalidOperationException("Insert/Update failed, no Id returned.");

        //    model.Id = Convert.ToInt32(result);
        //    return model;
        //}

        public async Task<bool> DeleteAsync(int id, int updatedBy)
        {
            await using var conn = _factory.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.DeleteDropdownMasterById";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
            cmd.Parameters.Add(new SqlParameter("@UpdatedBy", SqlDbType.Int) { Value = updatedBy });

            var result = await cmd.ExecuteScalarAsync();
            return result != null && Convert.ToInt32(result) > 0;
        }

        private static DropdownMaster MapDropdown(SqlDataReader reader)
        {
            return new DropdownMaster
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"] as string ?? string.Empty,
                Value = reader["Value"] as string ?? string.Empty,
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"]),
                UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : (int?)reader["UpdatedBy"],
                UpdatedDateTime = reader["UpdatedDateTime"] == DBNull.Value ? null : (DateTime?)reader["UpdatedDateTime"]
            };
        }
    }
}
