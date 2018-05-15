using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Managers;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Models.Entity;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Models.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Controllers
{
    /// <summary>
    /// Provides methods to add/remove feature groups and to assign users to these groups.
    /// </summary>
    [Route("Api/[controller]")]
    public class FeatureGroupsController : Controller
    {
        private readonly FeatureGroupsManager _manager;

        public FeatureGroupsController(FeatureGroupsManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Gets all feature groups.
        /// </summary>
        [Authorize("read:featuretoggle")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FeatureGroupResult>), 200)]
        [ProducesResponseType(403)]
        public IActionResult GetAll()
        {
            if (!UserPermissions.IsAllowedToAdminister(User.Identity))
                return Forbid();

            var groups = _manager.GetAllGroups(loadMembers: true, loadFeatures: true);
            var results = groups.ToList().Select(g => new FeatureGroupResult(g)); // note: ToList() is required here
            return Ok(results);
        }

        /// <summary>
        /// Gets a specific feature group by ID.
        /// </summary>
        [Authorize("read:featuretoggle")]
        [HttpGet("{groupId}")]
        [ProducesResponseType(typeof(FeatureGroupResult), 200)]
        [ProducesResponseType(403)]
        public IActionResult GetById(int groupId)
        {
            if (!UserPermissions.IsAllowedToAdminister(User.Identity))
                return Forbid();

            var group = _manager.GetGroup(groupId, loadMembers: true, loadFeatures: true);

            if (group == null)
                return NotFound();

            return Ok(new FeatureGroupResult(group));
        }

        /// <summary>
        /// Stores a new feature group.
        /// </summary>
        /// <param name="groupArgs">Creation arguments</param>
        [Authorize("write:featuretoggle")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        public IActionResult Create([FromBody]FeatureGroupArgs groupArgs)
        {
            if (!UserPermissions.IsAllowedToAdminister(User.Identity))
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var group = _manager.CreateGroup(groupArgs);
                return Created($"{Request.Scheme}://{Request.Host}/api/FeatureGroups/{group.Id}", group.Id);
            }
            catch (ResourceNotFoundException<Feature> e)
            {
                return StatusCode(422, e.Message); // Invalid feature ID
            }
            catch (ArgumentException e)
            {
                return StatusCode(409, e.Message); // Group with that name already exists
            }
        }

        /// <summary>
        /// Deletes a feature group. Members are moved to the default group.
        /// </summary>
        [Authorize("write:featuretoggle")]
        [HttpDelete("{groupId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public IActionResult Delete(int groupId)
        {
            if (!UserPermissions.IsAllowedToAdminister(User.Identity))
                return Forbid();

            try
            {
                _manager.RemoveGroup(groupId);
            }
            catch (ResourceNotFoundException<FeatureGroup> e)
            {
                return NotFound(e.Message); // group does not exist
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(409, e.Message); // tried to delete protected group
            }

            return NoContent();
        }

        /// <summary>
        /// Updates a feature group.
        /// </summary>
        [Authorize("write:featuretoggle")]
        [HttpPut("{groupId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult Update(int groupId, [FromBody]FeatureGroupArgs groupArgs)
        {
            if (!UserPermissions.IsAllowedToAdminister(User.Identity))
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _manager.UpdateGroup(groupId, groupArgs);
                return NoContent();
            }
            catch (ResourceNotFoundException<FeatureGroup> e)
            {
                return NotFound(e.Message); // invalid group ID
            }
            catch (ResourceNotFoundException<Feature> e)
            {
                return StatusCode(422, e.Message); // invalid feature ID
            }
            catch (InvalidOperationException e)
            {
                // tried to rename protected group or tried to move users to the public group
                return StatusCode(409, e.Message);
            }
            catch (ArgumentException e)
            {
                return StatusCode(409, e.Message); // new group name already in use
            }
        }

        /// <summary>
        /// Removes a user from its current feature group and assigns it to a new feature group.
        /// </summary>
        /// <returns></returns>
        [Authorize("write:featuretoggle")]
        [HttpPut("/Api/Users/{userId}/FeatureGroup/{groupId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public IActionResult AssignMember(string userId, int groupId)
        {
            if (!UserPermissions.IsAllowedToAdminister(User.Identity))
                return Forbid();

            try
            {
                _manager.MoveUserToGroup(userId, groupId);
            }
            catch (ResourceNotFoundException<FeatureGroup> e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(409, e.Message); // tried to move user to public group
            }
            return NoContent();
        }
    }
}
