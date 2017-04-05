using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Managers;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Models.Entity;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Models.FeatureGroups;
using System;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Controllers
{
    /// <summary>
    /// Provides methods to add/remove feature groups and to assign users to these groups.
    /// </summary>
    [Route("api/[controller]")]
    public class FeatureGroupsController : Controller
    {
        private readonly FeatureGroupsManager _manager;

        public FeatureGroupsController(FeatureGroupsManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var groups = _manager.GetGroups(loadMembers: true, loadFeatures: true);
            var results = groups.Select(g => new FeatureGroupResult(g));
            return Ok(results);
        }

        [HttpGet("{groupId}")]
        public IActionResult GetById(int groupId)
        {
            var group = _manager.GetGroup(groupId, loadMembers: true, loadFeatures: true);

            if (group == null)
                return NotFound();

            return Ok(new FeatureGroupResult(group));
        }

        [HttpPost]
        public IActionResult Create([FromBody]FeatureGroupViewModel groupVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newGroup = new FeatureGroup { Name = groupVM.Name };

                newGroup.EnabledFeatures = _manager.GetFeatures(groupVM.EnabledFeatures)
                    .Select(f => new FeatureToFeatureGroupMapping(f, newGroup))
                    .ToList();

                _manager.AddGroup(newGroup);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{groupId}")]
        public IActionResult Delete(int groupId)
        {
            var success = _manager.RemoveGroup(groupId);

            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpPut("{groupId}")]
        public IActionResult Update(int groupId, [FromBody]FeatureGroupViewModel groupVM)
        {
            // TODO: What is the purpose of this operation?
            return BadRequest();
        }

        [HttpPut("/Users/{userId}/FeatureGroup/{groupId}")]
        public IActionResult AssignMember(string userId, int groupId)
        {
            var user = _manager.GetOrCreateUser(userId);
            var group = _manager.GetGroup(groupId, loadMembers: true);

            if (group == null)
                return NotFound($"There is no feature group with ID '{groupId}'");

            _manager.MoveUserToGroup(user, group);
            return Ok();
        }
    }
}
