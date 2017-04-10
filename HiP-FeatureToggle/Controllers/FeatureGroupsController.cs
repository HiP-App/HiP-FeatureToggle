using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Managers;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Models.Entity;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Models.FeatureGroups;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Services;
using PaderbornUniversity.SILab.Hip.Webservice;
using System;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Controllers
{
    /// <summary>
    /// Provides methods to add/remove feature groups and to assign users to these groups.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class FeatureGroupsController : Controller
    {
        private readonly FeatureGroupsManager _manager;
        private readonly CmsService _cmsService;

        public FeatureGroupsController(FeatureGroupsManager manager, CmsService cmsService)
        {
            _manager = manager;
            _cmsService = cmsService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery]string identity)
        {
            if (!IsAdministrator(identity))
                return Forbid();

            var groups = _manager.GetGroups(loadMembers: true, loadFeatures: true);
            var results = groups.ToList().Select(g => new FeatureGroupResult(g)); // note: ToList() is required here
            return Ok(results);
        }

        [HttpGet("{groupId}")]
        public IActionResult GetById([FromQuery]string identity, int groupId)
        {
            if (!IsAdministrator(identity))
                return Forbid();

            var group = _manager.GetGroup(groupId, loadMembers: true, loadFeatures: true);

            if (group == null)
                return NotFound();

            return Ok(new FeatureGroupResult(group));
        }

        [HttpPost]
        public IActionResult Create([FromQuery]string identity, [FromBody]FeatureGroupViewModel groupVM)
        {
            if (!IsAdministrator(identity))
                return Forbid();

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
        public IActionResult Delete([FromQuery]string identity, int groupId)
        {
            if (!IsAdministrator(identity))
                return Forbid();

            var success = _manager.RemoveGroup(groupId);

            if (!success)
                return NotFound();

            return Ok();
        }

        [HttpPut("{groupId}")]
        public IActionResult Update([FromQuery]string identity, int groupId, [FromBody]FeatureGroupViewModel groupVM)
        {
            if (!IsAdministrator(identity))
                return Forbid();

            // TODO: What is the purpose of this operation?
            return BadRequest();
        }

        [HttpPut("/api/Users/{userId}/FeatureGroup/{groupId}")]
        public IActionResult AssignMember([FromQuery]string identity, string userId, int groupId)
        {
            if (!IsAdministrator(identity))
                return Forbid();

            var user = _manager.GetOrCreateUser(userId);
            var group = _manager.GetGroup(groupId, loadMembers: true);

            if (group == null)
                return NotFound($"There is no feature group with ID '{groupId}'");

            _manager.MoveUserToGroup(user, group);
            return Ok();
        }

        private bool IsAdministrator(string identity)
        {
            return _cmsService.GetUserRole(identity ?? User.Identity.GetUserIdentity()) == "Administrator";
        }
    }
}
