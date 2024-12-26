using hookset_server.DBHelpers;
using hookset_server.JWTManager;
using hookset_server.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace hookset_server.Controllers
{
    [ApiController]
    [Route("posts")]
    public class PostController : ControllerBase
    {
        private readonly IUserDBHelper userDBHelper;
        private readonly IPostsDBHelper _postsDBHelper;
        private readonly ICommentsDBHelper _commentsDBHelper;


        public PostController(IUserDBHelper userDBHelper, IPostsDBHelper postsDBHelper, ICommentsDBHelper commentsDBHelper)
        {
            this.userDBHelper = userDBHelper;
            this._postsDBHelper = postsDBHelper;
            this._commentsDBHelper = commentsDBHelper;
        }

        [HttpPost("images")]
        public async Task<ActionResult<string>> createPostUpload([FromForm] IFormFile imageFile)
        {
            Console.WriteLine("Hit api");
            Console.WriteLine(imageFile.FileName);
            var imageFilePath = Path.GetTempFileName();

            using (var stream = System.IO.File.Create(imageFilePath))
            {
                await imageFile.CopyToAsync(stream);
            }

            var postImage= _postsDBHelper.uploadPostImages( new Guid() ,new BlobContentModel { fileName = $"{new Guid()}" });

            return Ok("");
        }

        [HttpPost]
        public async Task<ActionResult<Posts>> createPost(Guid userId, createPostDTO postCreationObj)
        {
            var user = await userDBHelper.getUser(null, userId);

            if (user == null) return StatusCode(403, "Not Authorized");





            var newPost = new insertPostDTO
            {
                userId = user.Id,
                userName = user.userName,
                likes = 0,
                createdDate = DateTime.Now,
                description = postCreationObj.description,
                bodyOfWaterCaughtIn = postCreationObj.bodyOfWaterCaughtIn,
                weight =  0,
                length = 0,
                fishSpecies = postCreationObj.fishSpecies,
                filePath = "",
            };


            var post = await _postsDBHelper.insertPost(newPost);

            if (post == null) return StatusCode(500, "Error creating post");

            return Ok(post);
        }

        [HttpGet("list-posts")]
        public async Task<ActionResult<PostDTO>> listPosts(Guid userId, int? perPage, int? page)
        {
            if (perPage > 100) return StatusCode(500, "Too many items perPage requested");
            var posts = await _postsDBHelper.listPosts(userId, page, perPage, false);
            if (posts == null) return StatusCode(404, "No posts found");
            return Ok(posts);
        }

        [HttpGet("post")]
        public async Task<ActionResult<PostDTO>> getPost(Guid postId)
        {
            try
            {
                var post = await _postsDBHelper.getPost(postId);
                if (post == null) return StatusCode(404, "Post not found");
                return Ok(post);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
  
        }

        [HttpPut("{postId}")]
        public async Task<ActionResult<PostDTO>> updatePost(Guid postId, String? comment, Boolean like)
        {
            if (comment != null && comment.Length > 250) return BadRequest("Comment is too long");
            var post = await _postsDBHelper.getPost(postId);

            if (post == null) return BadRequest();

            if (comment != null)
            {
                var commentDTO = new Comments
                {
                    Id = Guid.NewGuid(),
                    UserId = post.userId,
                    PostId = post.Id,
                    comment = comment,
                };

                var createdComment = await _commentsDBHelper.insertComment(commentDTO);
                if (createdComment == null) return NotFound();
                var postComments = await _commentsDBHelper.getPostComments(postId, null);
                post.comments = postComments.ToList();
                return Ok(post);
            }

            else if (like == true)
            {
                var likeDTO = new Likes
                {
                    Id = Guid.NewGuid(),
                    UserId = post.userId,
                    PostId = post.Id,
                };

                var createdLike = await _postsDBHelper.insertLike(likeDTO);
                if (createdLike != null) post.likes += 1;
                else post.likes -= 1;
                return Ok(post);
            }


            else return StatusCode(500, "Invalid parameters provided");
        }


    }
}
