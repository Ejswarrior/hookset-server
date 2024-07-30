using Dapper;
using hookset_server.models;
using Newtonsoft.Json;
using System.Data;

namespace hookset_server.DBHelpers
{

    public interface ICommentsDBHelper
    {
        string commentsQuery { get; }
        public Task<List<CommentDTO>> getPostComments(Guid postId, System.Data.IDbConnection? dbConnection);
        public Task<Comments?> insertComment(Comments comment);
    }
    public class CommentsDBHelper: ICommentsDBHelper
    {
        private readonly DapperContext _dapperContext;

        public readonly string commentsQuery = "SELECT Comments.Comment, HooksetUser.UserName FROM Comments LEFT JOIN HooksetUser ON Comments.UserId = HooksetUser.Id WHERE PostId = @PostId ";

        string ICommentsDBHelper.commentsQuery { get => commentsQuery; }

        public CommentsDBHelper(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        private async Task<List<CommentDTO>> getCommentsFromConnection(Guid postId, System.Data.IDbConnection? connection)
        {
            var postComments = await connection.QueryAsync<CommentDTO>(commentsQuery, new { PostId = postId });
            return postComments.ToList();
        }

        public async Task<List<CommentDTO>> getPostComments(Guid postId, System.Data.IDbConnection? dbConnection)
        {
            if(dbConnection == null)
            {
                using (var connection = _dapperContext.createConnection())
                {
                    return await getCommentsFromConnection(postId, connection);
                }
            }
            
            return await getCommentsFromConnection(postId, dbConnection);
         
            
        }

        public async Task<Comments?> insertComment(Comments comment)
        {
            var insertCommentQuery = "INSERT INTO Comments (Id,UserId,PostId,Comment) VALUES (@Id, @UserId, @PostId, @Comment);";

            using (var connection = _dapperContext.createConnection())
            {
                Console.WriteLine(JsonConvert.SerializeObject(comment));
                var createCommentParams = new { Id = comment.Id, UserId = comment.UserId, PostId = comment.PostId, Comment = comment.comment };
                var commentQuery = await connection.QuerySingleOrDefaultAsync<int>(insertCommentQuery, createCommentParams);
                Console.WriteLine(JsonConvert.SerializeObject(commentQuery));
                Console.WriteLine(commentQuery);
                return comment;
            }

        }

        Task<List<CommentDTO>> ICommentsDBHelper.getPostComments(Guid postId, IDbConnection? dbConnection)
        {
            throw new NotImplementedException();
        }

        Task<Comments?> ICommentsDBHelper.insertComment(Comments comment)
        {
            throw new NotImplementedException();
        }
    }
}
