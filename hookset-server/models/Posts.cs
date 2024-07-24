using System.Runtime.InteropServices;

namespace hookset_server.models
{
    
    public class createPostDTO
    {
        public string description { get; set; }

        public string bodyOfWaterCaughtIn { get; set; }

        public string fishSpecies { get; set; }
        public int? weight { get; set; }
        public int? length { get; set; }
    }

    public class insertPostDTO
    {
        public int userId { get; set; }
        public DateTime createdDate { get; set; }
        public int likes { get; set; }

        public string description { get; set; }

        public string userName { get; set; }
        public DateTime? updatedDate { get; set; }

        public string bodyOfWaterCaughtIn { get; set; }

        public string fishSpecies { get; set; }
        public int? weight { get; set; }
        public int? length { get; set; }
    }

    public class Posts
    {
        public int Id { get; set; }
        public int userId { get; set; }
        public DateTime createdDate { get; set; }
        public int likes { get; set; }

        public string description { get; set; }

        public string userName { get; set; }
        public DateTime? updatedDate { get; set; }

        public string bodyOfWaterCaughtIn { get; set; }

        public string fishSpecies { get; set; }
        public int? weight { get; set; }
        public int? length { get; set; }


    }
}
