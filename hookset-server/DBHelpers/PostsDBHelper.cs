using Dapper;
using hookset_server.models;
using Newtonsoft.Json;

namespace hookset_server.DBHelpers
{   
    public interface IPostsDBHelper
    {
        public Task<Posts?> insertPost(insertPostDTO postObj);
    }
    public class PostsDBHelper: IPostsDBHelper
    {
        private readonly DapperContext _dapperContext;

        public PostsDBHelper(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public Posts ConvertToPostDTO(Guid id, insertPostDTO postObj)
        {
            return new Posts
            {
                Id = id,
                userName = postObj.userName,
                userId = postObj.userId,
                createdDate = postObj.createdDate,
                weight = postObj.weight,
                length = postObj.length,
                fishSpecies = postObj.fishSpecies,
                description = postObj.description,
                bodyOfWaterCaughtIn = postObj.bodyOfWaterCaughtIn
            };
        }

        public async Task<Posts?> insertPost(insertPostDTO postObj)  
        {
            var createPostQuery = "INSERT INTO Posts (Id,UserId,UserName,CreatedDate,Likes,Description,BodyOfWaterCaughtIn,Weight,Length,FishSpecies,UpdatedDate) VALUES (@Id, @UserId, @UserName, @CreatedDate, @Likes, @Description, @BodyOfWaterCaughtIn, @Weight, @Length, @FishSpecies, @UpdatedDate);";
            Console.Write(JsonConvert.SerializeObject(postObj));
            var newID = Guid.NewGuid();
            Console.WriteLine(newID); 

            using (var connection = _dapperContext.createConnection())
            {
                var createPostParameters = new { Id = newID, UserId = postObj.userId, UserName = postObj.userName, CreatedDate = postObj.createdDate, Likes = postObj.likes, Description = postObj.description, BodyOfWaterCaughtIn = postObj.bodyOfWaterCaughtIn, Weight = postObj.weight ?? null, Length = postObj.length ?? null, FishSpecies = postObj.fishSpecies ?? null, UpdatedDate = postObj.updatedDate ?? null };

                var id = await connection.QuerySingleOrDefaultAsync<int>(createPostQuery, createPostParameters);
                var createdPost = ConvertToPostDTO(newID, postObj);
                return createdPost;
            }

        }
    }
}
