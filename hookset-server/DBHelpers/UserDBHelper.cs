using Dapper;
using hookset_server.models;
using hookset_server.QueryBuilders;
using Microsoft.OpenApi.Validations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace hookset_server.DBHelpers
{
    public interface IUserDBHelper
    {
        public Task<User?> getUser(string? username, Guid? userId);
        public Task<User> createUser(UserCreateDTO userCreate);
    }

    public class UserCreateDTO
    {
        [Required]
        [NotNull]
        public string email { get; set; }
        [Required]
        [NotNull]
        public string password { get; set; }
        [Required]
        [NotNull]
        public string firstName { get; set; }
        [Required]
        [NotNull]
        public string lastName { get; set; }

        [Required]
        public string userName { get; set; }
    };
    public class UserDBHelper : IUserDBHelper
    {
        private readonly DapperContext _dapperContext;

        public UserDBHelper(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        async public Task<User?> getUser(string? email = null, Guid? userId = null)
        {
            
            var emailQuery = new SelectQueryBuilder().addTableName("HooksetUser").getWhereValues(new List<WhereQueries> { new WhereQueries { paramName = "Email", sqlName = "Email" } }).buildSelectQuery();
            var idQuery = new SelectQueryBuilder().addTableName("HooksetUser").getWhereValues(new List<WhereQueries> { new WhereQueries { paramName = "UserId", sqlName = "Id" } }).buildSelectQuery();

            using (var connection = _dapperContext.createConnection())
            {
                var user = await connection.QuerySingleOrDefaultAsync<User>(email != null ? emailQuery : idQuery, email != null? new {  email } : new { userId });
                return user;
            }
        }

        async public Task<User> createUser(UserCreateDTO userCreate)
        {
            var createUserQuery = new InsertQueryBuilder().addTableName("HooksetUser").addColumnNames(new[] { "Id", "Email", "Password", "FirstName", "lastName", "UserName" }).addParamNames(new[] { "Id", "Email", "Password", "FirstName", "LastName", "UserName" }).buildInsertQuery(true);

            using (var connection = _dapperContext.createConnection())
            {
                var userID = Guid.NewGuid();
                var id = await connection.QueryAsync(createUserQuery, new { Id = userID,  Email = userCreate.email, Password = userCreate.password, FirstName = userCreate.firstName, LastName = userCreate.lastName, UserName = userCreate.userName });
                var user = new User
                {
                    Id = userID,
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
