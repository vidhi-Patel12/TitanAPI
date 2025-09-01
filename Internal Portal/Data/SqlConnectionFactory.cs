using Microsoft.Data.SqlClient;

namespace Internal_Portal.Data
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateConnection();
    }

    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly IConfiguration _config;
        public SqlConnectionFactory(IConfiguration config) => _config = config;

        public SqlConnection CreateConnection()
        {
            var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            return conn;
        }
    }
}