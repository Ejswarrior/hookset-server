namespace hookset_server.models
{
    public class UserRelationships
    {

        public Guid Id { get; set; }
        public Guid userId { get; set; }
        public Guid followedUserId { get; set; }

        public DateTime FollowingSince { get; set; }
    }

    public class UserRelationsDTO
    {
        public Guid Id { get; set; }
        public DateTime FollowingSince { get; set; }
        public string UserName { get; set; }
    }

}
