using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace hookset_server.models
{
    public class User
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string email { get; set; }
        [Required]
        [MaxLength(50)]
        public string userName { get; set; }
        [Required]
        [MaxLength(50)]
        public string firstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string lastName { get; set; }
        [Required]
        [MaxLength(250)]
        public string password { get; set;}
    }
}
