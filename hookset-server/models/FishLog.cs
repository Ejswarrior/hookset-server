using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace hookset_server.models
{
    public class FishLog
    {
        public Guid Id { get; set; }
        [AllowNull]

        public int? weight { get; set; }
        [AllowNull]

        public int? length { get; set; }
        [Required]
        [MaxLength(50)]
        public string fishSpecies { get; set; }
        [Required]
        [MaxLength(75)]
        public string bodyOfWaterCaughtIn { get; set; }

        public Guid userId { get; set; }
        [AllowNull]
        public Guid? postId { get; set; }
    }

    public class InsertFishLogDTO
    {
        [AllowNull]
        public int? weight { get; set; }
        [AllowNull]
        public int? length { get; set; }
        [Required]
        [MaxLength(50)]
        [NotNull]
        public string fishSpecies { get; set; } = string.Empty;
        [Required]
        [MaxLength(75)]
        public string bodyOfWaterCaughtIn { get; set; } = string.Empty;
        [Required]
        public Guid userId { get; set; }
        [AllowNull]
        public Guid? postId { get; set; }
    }
}
