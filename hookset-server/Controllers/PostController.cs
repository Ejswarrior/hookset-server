using hookset_server.DBHelpers;
using hookset_server.JWTManager;
using hookset_server.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hookset_server.Controllers
{
    [ApiController]
    [Route("posts")]
    public class PostController : ControllerBase
    {
        private readonly IUserDBHelper userDBHelper;
        private readonly IPostsDBHelper _postsDBHelper;


        public PostController( IUserDBHelper userDBHelper, IPostsDBHelper postsDBHelper)
        {
            this.userDBHelper = userDBHelper;
            this._postsDBHelper = postsDBHelper;
        }
        [HttpPost]
        public async Task<ActionResult<Posts>> createPost(Guid userId, createPostDTO postCreationObj)
        {
            var user = await userDBHelper.getUser(null, userId);

            if (user == null) return StatusCode(403, "Not Authorized");
            Console.Write(user.Id);
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
            };

            var post = await _postsDBHelper.insertPost(newPost);

            if (post == null) return StatusCode(500, "Error creating post");

            return Ok(post);
        }
    }
}
