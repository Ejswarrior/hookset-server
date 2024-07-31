using Dapper;
using hookset_server.models;

namespace hookset_server.DBHelpers
{
    public interface IUserRelationsDBHelper
    {
        public Task<UserRelationsDTO?> getUserRelationship(Guid userId, Guid followedUserId);
        public Task<UserRelationships> createUserRelationship(Guid userId, Guid followedUserId);
        public  Task<UserRelationships?> deleteUserRelationship(Guid userId, Guid folowedUserId);
        public Task<List<UserRelationsDTO>> listFollowing(Guid userId);
        public  Task<List<UserRelationsDTO>> listFollowers(Guid userId);
    }

    public class UserRelationsDBHelper: IUserRelationsDBHelper
    {

        private readonly DapperContext _dapperContext;


        public UserRelationsDBHelper(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<UserRelationsDTO?> getUserRelationship(Guid userId, Guid followedUserId)
        {
            const string queryUserRelationship = "SELECT UserRelationships.Id, UserRelationships.FollowingSince, HooksetUser.UserName FROM UserRelationships LEFT JOIN HooksetUser ON UserRelationships.FollowedUserId = HooksetUser.Id WHERE UserId = @UserId AND FollowedUserId = @FollowedUserId;";
            var queryUserRelationshipParams = new {UserId = userId.ToString(), FollowedUserId = followedUserId.ToString()};
            using (var conneciton = _dapperContext.createConnection())
            {
                var userRelationship = await conneciton.QuerySingleOrDefaultAsync<UserRelationsDTO>(queryUserRelationship, queryUserRelationshipParams);
                return userRelationship;
            }
        }

        public async Task<UserRelationships> createUserRelationship(Guid userId, Guid followedUserId)
        {

            const string createUserRelationshipQuery = "INSERT INTO UserRelationships (Id, UserId, FollowedUserId, FollowingSince) VALUES (@Id, @UserId, @FollowedUserId, @FollowingSince) SELECT SCOPE_IDENTITY();";
            var createUserRelationshipParams = new {Id = Guid.NewGuid(), UserId = userId.ToString(), FollowedUserId = followedUserId.ToString(), FollowingSince = DateTime.Now};

            using(var conneciton = _dapperContext.createConnection()) 
            {
                var id = await conneciton.QuerySingleOrDefaultAsync<Guid>(createUserRelationshipQuery, createUserRelationshipParams);

                var createdUser = new UserRelationships { Id = id, userId = userId, followedUserId = followedUserId, FollowingSince = DateTime.Now};

                return createdUser;
            }
            
        }

        public async Task<List<UserRelationsDTO>> listFollowing(Guid userId)
        {
            const string listFollowingQuery = "SELECT UserRelationships.Id, UserRelationships.FollowingSince, HooksetUser.UserName FROM UserRelationships LEFT JOIN HooksetUser ON UserRelationships.FollowedUserId = HooksetUser.Id WHERE UserId = @UserId";

            using (var connection = _dapperContext.createConnection())
            {
                var userRelations = await connection.QueryAsync<UserRelationsDTO>(listFollowingQuery, new { userId = userId });

                return userRelations.ToList();
            }
        }


        public async Task<List<UserRelationsDTO>> listFollowers(Guid userId)
        {
            const string listFollowingQuery = "SELECT UserRelationships.Id, UserRelationships.FollowingSince, HooksetUser.UserName FROM UserRelationships LEFT JOIN HooksetUser ON UserRelationships.FollowedUserId = HooksetUser.Id WHERE FollowedUserId = @UserId";

            using (var connection = _dapperContext.createConnection())
            {
                var userRelations = await connection.QueryAsync<UserRelationsDTO>(listFollowingQuery, new { userId = userId });

                return userRelations.ToList();
            }
        }


        public async Task<UserRelationships?> deleteUserRelationship(Guid userId, Guid folowedUserId)
        {
            const string doesUserRelationshipExistQuery = "SELECT * FROM UserRelationships WHERE UserId = @UserId AND FollowedUserId = @FollowedUserId;";
            var queryUserRelationshipParams = new { UserId = userId, FollowedUserId = folowedUserId };

            using (var conneciton = _dapperContext.createConnection())
            {
                var userRelationship = await conneciton.QuerySingleOrDefaultAsync<UserRelationships>(doesUserRelationshipExistQuery, queryUserRelationshipParams);

                if (userRelationship != null)
                {

                    const string deleteUserRelationshipQuery = "DELETE FROM UserRelationships WHERE UserId = @UserId AND FollowedUserId = @FollowedUserId";
                    var deleteUserRelationship = await conneciton.QuerySingleOrDefaultAsync<UserRelationships>(deleteUserRelationshipQuery, queryUserRelationshipParams);
                    return userRelationship;
                }

                return null;
            }
        }
    }
}
