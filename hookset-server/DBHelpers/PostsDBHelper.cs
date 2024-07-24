using Dapper;
using hookset_server.models;

namespace hookset_server.DBHelpers
{   
    public interface IPostsDBHelper
    {
        public Task<Posts> insertPost(insertPostDTO postObj);
    }
    public class PostsDBHelper: IPostsDBHelper
    {
        private readonly DapperContext _dapperContext;

        public PostsDBHelper(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

      

        public async Task<Posts?> insertPost(insertPostDTO postObj)  
        {
            var createPostQuery = "INSERT INTO Posts (Id,UserId,UserName,CreatedDate,Likes,Description,BodyOfWaterCaughtIn, Weight, Length, FishSpecies, ) VALUES (@Id, @UserId, @UserName, @CreatedDate, @Likes, @Description, @BodyOfWaterCaughtIn, @Weight,@Length, @FishSpecies); SELECT Scope_Identity();";

            using (var connection = _dapperContext.createConnection())
            {
                var createPostParameters = new { Id = Guid.NewGuid(), UserId = postObj.userId, UserName= postObj.userName, CreatedDate = postObj.createdDate, Likes = postObj.likes, Description = postObj.description, BodyOfWaterCaughtIn = postObj.bodyOfWaterCaughtIn, Weight = postObj.weight, Length = postObj.length, FishSpecies = postObj.fishSpecies};

                var id = await connection.QuerySingleAsync<int>(createPostQuery, createPostParameters);
                var createdPost = new Posts { Id = id };
                return createdPost;
            }

        }
    }
}
