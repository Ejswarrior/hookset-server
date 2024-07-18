using System.Data.SqlClient;
using System.Data;

namespace hookset_server.DBHelpers
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;


        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public IDbConnection createConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
