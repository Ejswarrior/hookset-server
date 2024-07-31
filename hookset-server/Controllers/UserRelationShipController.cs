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
        public async Task<ActionResult<UserRelationships>> getUserRelation(Guid userId, Guid followedUserId)
        {
            if (userId == followedUserId) return BadRequest();
            var userRelationship = await _UserRelationsHelper.getUserRelationship(userId,followedUserId);

            if(userRelationship == null) return NotFound();

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
    }
}
