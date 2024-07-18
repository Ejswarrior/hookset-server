using Dapper;
using hookset_server.models;
using Microsoft.OpenApi.Validations;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace hookset_server.DBHelpers
{
    public interface IUserDBHelper
    {
        public Task<User> getUser(string username);
        public Task<User> createUser(UserCreateDTO userCreate);
    }

    public class UserCreateDTO
    {
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
    };
    public class UserDBHelper : IUserDBHelper
    {
        private readonly DapperContext _dapperContext;

        public UserDBHelper(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        async public Task<User> getUser(string email)
        {
            var query = "SELECT * FROM User WHERE Email = @Email";


            using (var connection = _dapperContext.createConnection())
            {
                Console.WriteLine("Connected");
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new {  email });
                return user;
            }
        }

        async public Task<User> createUser(UserCreateDTO userCreate)
        {
            var query = "INSERT INTO User (Email, Password, FirstName, LastName) VALUES (@Email, @Password, @FirstName, @LastName)" + "SELECT CAST(SCOPE_IDENTITY() as int)";

            var parameters = new DynamicParameters();
            parameters.Add("Email", userCreate.email);
            parameters.Add("Password", userCreate.password);
            parameters.Add("FirstName", userCreate.firstName);
            parameters.Add("LastName", userCreate.lastName);

            using (var connection = _dapperContext.createConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);
                var user = new User
                {
                    Id = id,
                    email = userCreate.email,
                    password = userCreate.password,
                    firstName = userCreate.firstName,
                    lastName = userCreate.lastName,
                };

                return user;
            }
        }
    }
}
