namespace hookset_server.models
{
    public class Comments
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string comment { get; set; }

    }
}
