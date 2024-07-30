using Dapper;
using hookset_server.models;
using Newtonsoft.Json;
using System.Collections;
using System.Text;
using System.Xml.Linq;
using System.Text.Json;
using Microsoft.Extensions.Hosting;


namespace hookset_server.DBHelpers
{   
    public interface IPostsDBHelper
    {
        public Task<Posts?> insertPost(insertPostDTO postObj);
        public Task<List<PostDTO>> listPosts(Guid userId, int? pageStart, int? perPage, bool? follower);
        public Task<PostDTO?> getPost(Guid postId);
        public Task<String> constructListQuery(System.Data.IDbConnection connection, Guid userId, int? pageStart, int? perPage, bool? followers);
        public PostDTO ConvertToPostDTO(Posts postObj, List<CommentDTO> comments, int Likes);
        public  Task<Likes?> insertLike(Likes like);
    }
    public class PostsDBHelper: IPostsDBHelper
    {
        private readonly DapperContext _dapperContext;
        private readonly ICommentsDBHelper _commentsDBHelper;
        private readonly String likesQuery = "SELECT Count(*) FROM Likes WHERE PostId = @PostId";

        public PostsDBHelper(DapperContext dapperContext, ICommentsDBHelper commentsDBHelper)
        {
            _dapperContext = dapperContext;
            _commentsDBHelper = commentsDBHelper;
        }

        public Posts ConvertToPost(Guid id, insertPostDTO postObj)
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

        public PostDTO ConvertToPostDTO(Posts postObj, List<CommentDTO> comments, int Likes) {
            return new PostDTO
            {
                Id = postObj.Id,
                userName = postObj.userName,
                userId = postObj.userId,
                createdDate = postObj.createdDate,
                weight = postObj.weight,
                length = postObj.length,
                fishSpecies = postObj.fishSpecies,
                description = postObj.description,
                bodyOfWaterCaughtIn = postObj.bodyOfWaterCaughtIn,
                comments = comments != null ? comments.ToList() : [],
                likes = Likes,
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
                var createdPost = ConvertToPost(newID, postObj);
                return createdPost;
            }

        }


        public async Task<PostDTO?> getPost(Guid postId)
        {
            var getPostQuery = "SELECT * FROM Posts WHERE Id = @PostId;";
            using (var connection = _dapperContext.createConnection())
            {
                var post = await connection.QueryFirstOrDefaultAsync<Posts>(getPostQuery, new { PostId = postId});
                if (post == null) return null;

                var postComments = await _commentsDBHelper.getPostComments(postId, connection);
                var postLikes = await connection.QuerySingleAsync<int>(likesQuery, new { PostId = post.Id });
                return ConvertToPostDTO(post, postComments.ToList(), postLikes);
            }
        }

        public async Task<String> constructListQuery(System.Data.IDbConnection connection , Guid userId, int? pageStart, int? perPage, bool? followers)
        {
            var listPostQuery = "SELECT * FROM Posts";

            if (followers != null && followers == false) listPostQuery += " WHERE UserId = @UserId";

            if (followers == true)
            {
                var followingIds = await connection.QueryAsync<UserRelationships>("Select DISTINCT UserTwoId FROM Followers WHERE UserId = @UserId & UserOneFollowUserTwo = 1");
                listPostQuery += " Where UserId IN @Ids";
            }
            if (pageStart != null && perPage != null && pageStart != 0) listPostQuery += $" ORDER BY CreatedDate DESC OFFSET @perPage * @pageStart ROWS FETCH NEXT @perPage ROWS ONLY";
            if (pageStart == null || perPage == null || pageStart == 0) listPostQuery += " ORDER BY CreatedDate DESC";
            listPostQuery += ";";
            return listPostQuery;
        }

        public async Task<List<PostDTO>> listPosts(Guid userId, int? pageStart, int? perPage, bool? followers)
        {
            //todo: create better configuration for pulling different lists of posts
            using (var connection = _dapperContext.createConnection())
            {

                var listPostQuery = await constructListQuery(connection, userId, pageStart, perPage, followers);  
           
                Console.WriteLine(listPostQuery);
                var posts = await connection.QueryAsync<Posts>(listPostQuery, new {UserId = userId, perPage = perPage, pageStart = pageStart });
                List<PostDTO> postDTOs = new List<PostDTO>();
                if (posts.Count() == 0) return [];

                if (posts != null)
                {
                    foreach (var post in posts)
                    {
                        if (post != null)
                        {
                            var postComments = await _commentsDBHelper.getPostComments(post.Id, connection);
                            var postLikes = await connection.QuerySingleAsync<int>(likesQuery, new { PostId = post.Id });
                            Console.Write(JsonConvert.SerializeObject(postComments));

                            var postDto = ConvertToPostDTO(post, postComments.ToList(), postLikes);

                            postDTOs.Add(postDto);
                        }
                    }
                }
                return postDTOs;
            }
        }


        public async Task<Likes?> insertLike(Likes like)
        {
            var insertLikeQuery = "INSERT into Likes (Id, UserId, PostId) VALUES (@Id, @UserId, @PostId);";
            var deleteLikeQuery = "DELETE FROM Likes WHERE UserId = @UserId AND PostId = @PostId;";
            using (var connection = _dapperContext.createConnection())
            {
                var findLikeQuery = "SELECT * FROM Likes WHERE UserId = @UserId AND PostId = @PostId;";
                var basicQueryParams = new { UserId = like.UserId, PostId = like.PostId };
                var insertQueryParams = new { Id = like.Id, PostId = like.PostId, UserId = like.UserId };

                var existingLike = await connection.QuerySingleOrDefaultAsync<Likes>(findLikeQuery, basicQueryParams);
                var likeUpdateQuery = existingLike != null ? insertLikeQuery : deleteLikeQuery;
                var likeQuery = await connection.QuerySingleOrDefaultAsync<Likes>(insertLikeQuery, existingLike != null ? insertQueryParams : basicQueryParams);
                if (likeQuery == null) return null;
                return like;
            }

        }
    }
}
