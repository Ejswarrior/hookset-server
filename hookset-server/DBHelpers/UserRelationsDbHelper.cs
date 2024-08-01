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

    public class UserRelationsQueries
    {
        public readonly string queryUserRelationship = "SELECT UserRelationships.Id, UserRelationships.FollowingSince, HooksetUser.UserName FROM UserRelationships LEFT JOIN HooksetUser ON UserRelationships.FollowedUserId = HooksetUser.Id WHERE UserId = @UserId AND FollowedUserId = @FollowedUserId;";
        public readonly string createUserRelationshipQuery = "INSERT INTO UserRelationships (Id, UserId, FollowedUserId, FollowingSince) VALUES (@Id, @UserId, @FollowedUserId, @FollowingSince) SELECT SCOPE_IDENTITY();";
        public readonly string listFollowingQuery = "SELECT UserRelationships.Id, UserRelationships.FollowingSince, HooksetUser.UserName FROM UserRelationships LEFT JOIN HooksetUser ON UserRelationships.FollowedUserId = HooksetUser.Id WHERE UserId = @UserId";
        public readonly string doesUserRelationshipExistQuery = "SELECT * FROM UserRelationships WHERE UserId = @UserId AND FollowedUserId = @FollowedUserId;";
        public readonly string deleteUserRelationshipQuery = "DELETE FROM UserRelationships WHERE UserId = @UserId AND FollowedUserId = @FollowedUserId";
    }

    public class UserRelationsDBHelper: IUserRelationsDBHelper
    {

        private readonly DapperContext _dapperContext;
        private readonly UserRelationsQueries _userRelationQueries;


        public UserRelationsDBHelper(DapperContext dapperContext, UserRelationsQueries userRelationsQueries)
        {
            _dapperContext = dapperContext;
            _userRelationQueries = userRelationsQueries;
        }

        public async Task<UserRelationsDTO?> getUserRelationship(Guid userId, Guid followedUserId)
        {
            var queryUserRelationshipParams = new {UserId = userId.ToString(), FollowedUserId = followedUserId.ToString()};
            using (var conneciton = _dapperContext.createConnection())
            {
                var userRelationship = await conneciton.QuerySingleOrDefaultAsync<UserRelationsDTO>(_userRelationQueries.queryUserRelationship, queryUserRelationshipParams);
                return userRelationship;
            }
        }

        public async Task<UserRelationships> createUserRelationship(Guid userId, Guid followedUserId)
        {

            var createUserRelationshipParams = new {Id = Guid.NewGuid(), UserId = userId.ToString(), FollowedUserId = followedUserId.ToString(), FollowingSince = DateTime.Now};

            using(var conneciton = _dapperContext.createConnection()) 
            {
                var id = await conneciton.QuerySingleOrDefaultAsync<Guid>(_userRelationQueries.createUserRelationshipQuery, createUserRelationshipParams);

                var createdUser = new UserRelationships { Id = id, userId = userId, followedUserId = followedUserId, FollowingSince = DateTime.Now};

                return createdUser;
            }
            
        }

        public async Task<List<UserRelationsDTO>> listFollowing(Guid userId)
        {
            using (var connection = _dapperContext.createConnection())
            {
                var userRelations = await connection.QueryAsync<UserRelationsDTO>(_userRelationQueries.listFollowingQuery, new { userId = userId });

                return userRelations.ToList();
            }
        }


        public async Task<List<UserRelationsDTO>> listFollowers(Guid userId)
        {
            using (var connection = _dapperContext.createConnection())
            {
                var userRelations = await connection.QueryAsync<UserRelationsDTO>(_userRelationQueries.listFollowingQuery, new { userId = userId });

                return userRelations.ToList();
            }
        }


        public async Task<UserRelationships?> deleteUserRelationship(Guid userId, Guid folowedUserId)
        {
            var queryUserRelationshipParams = new { UserId = userId, FollowedUserId = folowedUserId };

            using (var conneciton = _dapperContext.createConnection())
            {
                var userRelationship = await conneciton.QuerySingleOrDefaultAsync<UserRelationships>(_userRelationQueries.doesUserRelationshipExistQuery, queryUserRelationshipParams);

                if (userRelationship != null)
                {
                    var deleteUserRelationship = await conneciton.QuerySingleOrDefaultAsync<UserRelationships>(_userRelationQueries.deleteUserRelationshipQuery, queryUserRelationshipParams);
                    return userRelationship;
                }

                return null;
            }
        }
    }
}
