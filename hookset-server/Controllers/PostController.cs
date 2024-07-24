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
        public async Task<ActionResult<Posts>> createPost(int userId, createPostDTO postCreationObj)
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
                weight = postCreationObj.weight != null ? postCreationObj.weight : null,
                length = postCreationObj.length != null ? postCreationObj.length : null,
                fishSpecies = postCreationObj.fishSpecies,
            };

            var post = await _postsDBHelper.insertPost(newPost);
        }
    }
}
