using Dapper;
using hookset_server.models;
using Newtonsoft.Json;
using System.Collections;

namespace hookset_server.DBHelpers
{   
    public interface IPostsDBHelper
    {
        public Task<Posts?> insertPost(insertPostDTO postObj);
        public Task<List<PostDTO>> listPosts(int? pageStart, int? perPage, Guid? userId, bool? follower);
        public Task<Posts?> getPost(Guid userId);
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

        public async Task<Posts?> getPost(Guid userId)
        {
            var getPostQuery = "SELECT * FROM Posts Where UserId = @UserId;";
            using (var connection = _dapperContext.createConnection())
            {
                var post = await connection.QueryFirstOrDefaultAsync<Posts>(getPostQuery);

                return post;
            }
        }

        public async Task<List<PostDTO>> listPosts(int? pageStart, int? perPage, Guid? userId, bool? followers)
        {
            var listPostQuery = "SELECT * FROM Posts";
            
            if (userId != null) listPostQuery += " WHERE UserId = @UserId";

            if (pageStart != null && perPage != null && pageStart != 0) listPostQuery = $" OFFSET {perPage} * {pageStart} ROWS FETCH {perPage} ROWS ONLY";

            listPostQuery += " ORDER BY CreatedDate DESC";
            listPostQuery += ";";
            //todo: create better configuration for pulling different lists of posts
            using (var connection = _dapperContext.createConnection())
            {
                var posts = await connection.QueryAsync<Posts>(listPostQuery, new {UserId  = userId});
                List<PostDTO> postDTOs = new List<PostDTO>();
                if (posts.Count() == 0) return [];

                if (posts != null)
                {
                    foreach (var post in posts)
                    {
                        if (post != null)
                        {
                            var commentsQuery = "SELECT * FROM Comments WHERE PostId = @PostId";
                            var likesQuery = "SELECT Count(*) FROM Likes WHERE PostId = @PostId";

                            var postComments = await connection.QueryAsync<Comments>(commentsQuery, new { PostId = post.Id });
                            var postLikes = await connection.QuerySingleAsync<int>(likesQuery, new { PostId = post.Id });

                            var postDto = new PostDTO
                            {
                                Id = post.Id,
                                userId = post.userId,
                                userName = post.userName,
                                fishSpecies = post.fishSpecies,
                                description = post.description,
                                length = post.length,
                                weight = post.weight,
                                createdDate = post.createdDate,
                                updatedDate = post.updatedDate,
                                likes = postLikes,
                                comments = postComments != null ? postComments.ToList() : [],
                            };

                            postDTOs.Add(postDto);
                        }
                    }
                }
                return postDTOs;
            }
        }
    }
}
