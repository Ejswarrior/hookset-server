namespace hookset_server.models
{
    public class PostImage
    {
        public Guid id { get; set; }
        public Guid postId { get; set; }
        public string imageUrl { get; set; }
        public string imageType { get; set; }
        public Guid fishLogId { get; set; }
    }

    public class SQLPostImage
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string ImageUrl { get; set; }
        public string ImageType { get; set; }
        public Guid FishLogId { get; set; }
    }
}
