namespace hookset_server.models
{
    public class Likes
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
    }
}
