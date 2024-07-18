using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace hookset_server.models
{
    public class User
    {
        public int Id { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }

        public string lastName { get; set; }

        public string password { get; set;}
    }
}
