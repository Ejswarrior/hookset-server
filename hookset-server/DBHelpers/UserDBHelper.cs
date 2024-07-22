using Dapper;
using hookset_server.models;
using Microsoft.OpenApi.Validations;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace hookset_server.DBHelpers
{
    public interface IUserDBHelper
    {
        public Task<User?> getUser(string username);
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

        async public Task<User?> getUser(string email)
        {
            var query = "SELECT * FROM HooksetUser WHERE Email = @Email;";


            using (var connection = _dapperContext.createConnection())
            {
                Console.WriteLine("Connected");
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new {  email });
                if (user == null)
                {
                    Console.WriteLine("No User");
                }
                return user;
            }
        }

        async public Task<User> createUser(UserCreateDTO userCreate)
        {
            var createUserQuery = "INSERT INTO HooksetUser (Id,Email,Password,FirstName,LastName) VALUES (@Id, @Email, @Password, @FirstName, @LastName);";

            using (var connection = _dapperContext.createConnection())
            {
                var id = await connection.QueryAsync(createUserQuery, new { Id = Guid.NewGuid(),  Email = userCreate.email, Password = userCreate.password, FirstName = userCreate.firstName, LastName = userCreate.lastName });
                var user = new User
                {
                    Id = 1,
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
