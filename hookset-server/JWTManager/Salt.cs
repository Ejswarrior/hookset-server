using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace hookset_server.JWTManager
{
    public class SaltManager
    {
        public string saltPassword(string password)
        {
           
            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password,workFactor: 10, HashType.SHA512);

            return hashedPassword;
        }

        public bool verifiySalt(string password, string saltedPassword) 
        {
            Console.WriteLine(password);
            return BCrypt.Net.BCrypt.EnhancedVerify(password, saltedPassword, HashType.SHA512);

        }
    }
}
