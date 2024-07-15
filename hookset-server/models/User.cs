using System.ComponentModel.DataAnnotations;

namespace hookset_server.models
{
    public class User
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string email { get; set; } = "";
        [Required]
        public string firstName { get; set; } = "";

        public string lastName { get; set; } = "";
    }
}
