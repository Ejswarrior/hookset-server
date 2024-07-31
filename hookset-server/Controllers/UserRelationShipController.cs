using hookset_server.DBHelpers;
using hookset_server.models;
using Microsoft.AspNetCore.Mvc;

namespace hookset_server.Controllers
{
    [ApiController]
    [Route("user-relationships/{userId}")]
    public class UserRelationShipController : ControllerBase
    {   
        private readonly IUserRelationsDBHelper _UserRelationsHelper;
        public UserRelationShipController(IUserRelationsDBHelper userRelationsDBHelper)
        { 
            _UserRelationsHelper = userRelationsDBHelper;
        }


        [HttpGet("{followedUserId}")]
        public async Task<ActionResult<UserRelationsDTO?>> getUserRelation(Guid userId, Guid followedUserId)
        {
            if (userId == followedUserId) return BadRequest();
            var userRelationship = await _UserRelationsHelper.getUserRelationship(userId,followedUserId);

            return Ok(userRelationship);

        }

        [HttpGet("following")]
        public async Task<ActionResult<List<UserRelationsDTO>>> getFollowingList(Guid userId)
        {
            var followingList = await _UserRelationsHelper.listFollowing(userId);

            if(followingList == null) return NotFound();

            return Ok(followingList);
        }

        [HttpGet("followers")]
        public async Task<ActionResult<List<UserRelationsDTO>>> getFollowersList(Guid userId)
        {
            var followingList = await _UserRelationsHelper.listFollowers(userId);

            if (followingList == null) return NotFound();

            return Ok(followingList);
        }

        [HttpPost("follow/{followerId}")]
        public async Task<ActionResult<UserRelationships>> followUser(Guid userId, Guid followerId)
        {
            if(userId == followerId) return BadRequest();

            var existingUserRelationship = await _UserRelationsHelper.getUserRelationship(userId, followerId);

            if (existingUserRelationship != null) return BadRequest();


            var createdUserRelationship = await _UserRelationsHelper.createUserRelationship(userId, followerId);

            if (createdUserRelationship == null) return StatusCode(500, "Issue creating user relationship");

            return createdUserRelationship;
        }

        [HttpPost("unfollow/{followerId}")]
        public async Task<ActionResult<UserRelationships>> unFollowUser(Guid userId, Guid followerId)
        {
            if (userId == followerId) return BadRequest();

            var existingUserRelationship = await _UserRelationsHelper.getUserRelationship(userId, followerId);

            if (existingUserRelationship == null) return BadRequest();


            var deletedUserRelationship = await _UserRelationsHelper.deleteUserRelationship(userId, followerId);

            if (deletedUserRelationship == null) return StatusCode(500, "Issue creating user relationship");

            return deletedUserRelationship;
        }



    }
}
