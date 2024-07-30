namespace hookset_server.models
{
    public class UserRelationships
    {

        public int Id { get; set; }
        public Guid userId { get; set; }
        public Guid userTwoId { get; set; }
        public int UserOneFollowUserTwo { get; set; }
        public int UserTwoFollowUserOne { get; set; }

        public DateTime FollowingSince { get; set; }
    }
}
