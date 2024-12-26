using Dapper;
using hookset_server.models;
using Newtonsoft.Json;
using hookset_server.QueryBuilders;



namespace hookset_server.DBHelpers
{   
    public interface IPostsDBHelper
    {
        public Task<Posts?> insertPost(insertPostDTO postObj);
        public Task<List<PostDTO>> listPosts(Guid userId, int? pageStart, int? perPage, bool? follower);
        public Task<PostDTO?> getPost(Guid postId);
        public PostDTO ConvertToPostDTO(PostsWithFishLog postObj, List<CommentDTO> comments, int Likes);
        public  Task<Likes?> insertLike(Likes like);

        public Task<List<SQLPostImage>> uploadPostImages(Guid postId, BlobContentModel fileData);
    }
    public class PostsDBHelper: IPostsDBHelper
    {
        private readonly DapperContext _dapperContext;
        private readonly ICommentsDBHelper _commentsDBHelper;
        private readonly String likesQuery = new SelectQueryBuilder().addTableName("Likes").addSelectValues(null, true).getWhereValues(new List<WhereQueries> { new WhereQueries { paramName = "PostId", sqlName = "PostId" } }).buildSelectQuery();
        private readonly IBlobStorageService _blobStorageService;

        public PostsDBHelper(DapperContext dapperContext, ICommentsDBHelper commentsDBHelper, IBlobStorageService blobStorageService)
        {
            _dapperContext = dapperContext;
            _commentsDBHelper = commentsDBHelper;
            _blobStorageService = blobStorageService;
        }

        public Posts ConvertToPost(Guid id, insertPostDTO postObj)
        {
            return new Posts
            {
                Id = id,
                userName = postObj.userName,
                userId = postObj.userId,
                createdDate = postObj.createdDate,
            };
        }

        public PostDTO ConvertToPostDTO(PostsWithFishLog postObj, List<CommentDTO> comments, int Likes) {
            return new PostDTO
            {
                Id = postObj.Id,
                userName = postObj.userName.Trim(),
                userId = postObj.userId,
                createdDate = postObj.createdDate,
                description = postObj.description,
                length = postObj.length,
                weight = postObj.weight,
                bodyOfWaterCaughtIn = postObj.bodyOfWaterCaughtIn,
                fishSpecies = postObj.fishSpecies,
                updatedDate = postObj.updatedDate, 
                comments = comments != null ? comments.ToList() : [],
                likes = Likes,
             };
        }

        public async Task<List<SQLPostImage>> uploadPostImages(Guid postId, BlobContentModel fileData)
        {
            var blobContent = new List<SQLPostImage>();

            
                try
                {

                    var url = await _blobStorageService.uploadBlob(fileData.fileName, fileData.filePath);
                    var extension = Path.GetExtension(fileData.fileName);

                    blobContent.Add(new SQLPostImage { Id = Guid.NewGuid(), PostId = postId, ImageType = $"image/{extension.Remove(0, 1)}", ImageUrl = url });
                }catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            

            return blobContent;
        }

        public async Task<FishLog?> insertFishLog(InsertFishLogDTO fishLogDto)
        {
            using (var connection = _dapperContext.createConnection()) {
                var createFishLogQuery = new InsertQueryBuilder().addTableName("FishLog").addColumnNames(new[] { "Id", "FishSpecies", "Weight", "Length", "BodyOfWaterCaughtIn", "PostId", "Likes", "Description" }).addParamNames(new[] { "Id", "FishSpecies", "Weight", "Length", "BodyOfWaterCaughtIn", "PostId", "UserId" }).buildInsertQuery(true);
                var createFishLogParameters = new { Id = Guid.NewGuid(), UserId = fishLogDto.userId, BodyOfWaterCaughtIn = fishLogDto.bodyOfWaterCaughtIn, Weight = fishLogDto.weight ?? null, Length = fishLogDto.length ?? null, FishSpecies = fishLogDto.fishSpecies };
                var fishLogId = await connection.QuerySingleOrDefaultAsync<Guid>(createFishLogQuery, createFishLogParameters);

                return new FishLog
                {
                    Id = fishLogId,
                    userId = fishLogDto.userId,
                    postId = fishLogId,
                    bodyOfWaterCaughtIn = fishLogDto.bodyOfWaterCaughtIn,
                    weight = fishLogDto.weight ?? null,
                    length = fishLogDto.length ?? null,
                    fishSpecies = fishLogDto.fishSpecies,
                };
            }
        }

        public async Task<Posts?> insertPost(insertPostDTO postObj)  
        {
            var createPostQuery = new InsertQueryBuilder().addTableName("Posts").addColumnNames(new[] { "Id", "UserId", "UserName", "CreatedDate", "Likes", "Description", "UpdatedDate" }).addParamNames(new[] { "Id", "UserId", "UserName", "CreatedDate", "Likes", "Description", "UpdatedDate" }).buildInsertQuery(true);
            Console.WriteLine(createPostQuery);
            var createFishLogQuery = new InsertQueryBuilder().addTableName("FishLog").addColumnNames(new[] { "Id", "FishSpecies", "Weight", "Length", "BodyOfWaterCaughtIn", "PostId", "UserId" }).addParamNames(new[] { "Id", "FishSpecies", "Weight", "Length", "BodyOfWaterCaughtIn", "PostId", "UserId" }).buildInsertQuery(true);
            Console.Write(JsonConvert.SerializeObject(postObj));
            var newID = Guid.NewGuid();
            var newFishLogId  = Guid.NewGuid();
            var images = await uploadPostImages(newID, new BlobContentModel { fileName = $"{postObj.userId}-{newID}"});



            using (var connection = _dapperContext.createConnection())
            {
                var insertImagesQuery = new InsertQueryBuilder().addTableName("PostImages").addColumnNames(new[] { "Id", "PostId", "ImageType", "ImageUrl", "FishLogId" }).addParamNames(new[] { "Id", "PostId", "ImageType", "ImageUrl", "FishLogId" }).buildInsertQuery(true);

                Console.WriteLine(insertImagesQuery);
                var createPostParameters = new { Id = newID, UserId = postObj.userId, UserName = postObj.userName, CreatedDate = postObj.createdDate, Likes = 0, Description = postObj.description, UpdatedDate = postObj.updatedDate ?? null };
                Console.WriteLine(createPostParameters);
                var postId = await connection.QuerySingleOrDefaultAsync<Posts>(createPostQuery, createPostParameters);
                var createFishLogParameters = new { Id = newFishLogId, UserId = postObj.userId, PostId = newID, BodyOfWaterCaughtIn = postObj.bodyOfWaterCaughtIn, Weight = postObj.weight ?? null, Length = postObj.length ?? null, FishSpecies = postObj.fishSpecies ?? null };
                var fishLogId = await connection.QuerySingleOrDefaultAsync<FishLog>(createFishLogQuery, createFishLogParameters);
                var postImages = new List<PostImage>();
                foreach(var image in images)
                {
                    var postImageId = await connection.QuerySingleOrDefaultAsync<PostImage>(insertImagesQuery, image);
                    postImages.Add(new PostImage { id = image.Id, fishLogId = image.FishLogId, imageType = image.ImageType, imageUrl = image.ImageUrl, postId = newID });

                }
                var createdPost = ConvertToPost(newID, postObj);
                return createdPost;
            }

        }


        public async Task<PostDTO?> getPost(Guid postId)
        {
            var getPostQuery =  new SelectQueryBuilder().addTableName("Posts").addLeftJoinValues(new[] {"Weight", "FishSpecies", "Length", "BodyOfWaterCaughtIn"}, "FishLog").addLeftJoin("FishLog", "PostId", "Posts.Id").getWhereValues(new List<WhereQueries> { new WhereQueries { paramName = "PostId", sqlName = "Id" } }).buildSelectQuery();
            using (var connection = _dapperContext.createConnection())
            {
                var post = await connection.QueryFirstOrDefaultAsync<PostsWithFishLog>(getPostQuery, new { PostId = postId});
                if (post == null) return null;

                var postComments = await _commentsDBHelper.getPostComments(postId, connection);
                var postLikes = await connection.QuerySingleAsync<int>(likesQuery, new { PostId = post.Id });
                return ConvertToPostDTO(post, postComments.ToList(), postLikes);
            }
        }

        private async Task<String> constructListQuery(System.Data.IDbConnection connection , Guid userId, int? pageStart, int? perPage, bool? followers)
        {
            var listPostQuery = "SELECT * FROM Posts FULL OUTER JOIN FishLog on Posts.id = Fishlog.PostId";

            if (followers != null && followers == false) listPostQuery += " WHERE Posts.UserId = @UserId";

            if (followers == true)
            {
                var followingIds = await connection.QueryAsync<UserRelationships>("Select DISTINCT UserTwoId FROM UserRelationships WHERE UserId = @UserId & UserOneFollowUserTwo = 1");
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
                var posts = await connection.QueryAsync<PostsWithFishLog>(listPostQuery, new {UserId = userId, perPage = perPage, pageStart = pageStart });
                List<PostDTO> postDTOs = new List<PostDTO>();
                if (posts.Count() == 0) return [];

                if (posts != null)
                {
                    foreach (var post in posts)
                    {
                        if (post != null)
                        {
                            Console.WriteLine("before comments");
                            var postComments = await _commentsDBHelper.getPostComments(post.Id, connection);
                            var postLikes = await connection.QuerySingleAsync<int>(likesQuery, new { PostId = post.Id });
                            Console.WriteLine("Before comments");

                            Console.Write(JsonConvert.SerializeObject(postComments));
                            Console.WriteLine("After comments");
                            var postDto = ConvertToPostDTO(post, postComments.ToList(), postLikes);

                            Console.WriteLine("After converting");
                            postDTOs.Add(postDto);
                        }
                    }
                }
                return postDTOs;
            }
        }


        public async Task<Likes?> insertLike(Likes like)
        {
            var insertLikeQuery = new InsertQueryBuilder().addColumnNames(new[] { "Id", "UserId", "PostId" }).addParamNames(new[] { "Id", "UserId", "PostId" }).buildInsertQuery(false);
            const string deleteLikeQuery = "DELETE FROM Likes WHERE UserId = @UserId AND PostId = @PostId;";
            using (var connection = _dapperContext.createConnection())
            {
                var findLikeQuery = new SelectQueryBuilder().addTableName("Likes").getWhereValues(new List<WhereQueries> { new WhereQueries { paramName = "UserId", sqlName = "UserId" }, new WhereQueries { paramName = "PostId", sqlName = "PostId" } }).buildSelectQuery();
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
