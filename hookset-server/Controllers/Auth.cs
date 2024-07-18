using hookset_server.JWTManager;
using hookset_server.models;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using hookset_server.DBHelpers;

namespace hookset_server.Controllers
{
    [ApiController]
    [Route("auth")]
    public class Auth : ControllerBase
    {

        private readonly IJWTManager jWTManager;
        private SaltManager _salt;
        private readonly IUserDBHelper userDBHelper;

        public Auth(IJWTManager jWTManager, SaltManager _salt, IUserDBHelper userDBHelper)
        {
            this.jWTManager = jWTManager;
            this._salt = _salt;
            this.userDBHelper = userDBHelper;
        }

        [HttpPost]
        [Route("/login")]
        public async Task<ActionResult> Login(string email, string password)
        {
            var authenticatedUser = await userDBHelper.getUser(email);
            
            if(authenticatedUser == null) return StatusCode(500, "Invalid Credentials");
           
            var encrytedPassword = _salt.verifiySalt(password, authenticatedUser.password);

            if (!encrytedPassword) return StatusCode(500, "Invalid Credentials");

            var token = jWTManager.Authenticate(email, password);
            
            return Ok(token);
        }

        [HttpPost]
        [Route("/create-acount")]

        public async Task<ActionResult> CreateAccount(string email, string password, string firstName, string lastName)
        {

            var duplicateUser = await userDBHelper.getUser(email);
            Console.WriteLine("Hit after get user");

            if (duplicateUser != null) return StatusCode(400, "User already exists");

            try
            {
                Console.WriteLine("Hit inside try");
                var hashedPassword = _salt.saltPassword(password);
                var createdUser = userDBHelper.createUser(new UserCreateDTO { email = email, password = password, firstName = firstName, lastName = lastName });
                if (createdUser == null) return StatusCode(500, "Error creating user");

                var token = jWTManager.Authenticate(email, password);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
