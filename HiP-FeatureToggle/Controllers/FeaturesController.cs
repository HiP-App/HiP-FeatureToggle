using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Managers;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Models.Entity;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Models.Rest;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Services;
using PaderbornUniversity.SILab.Hip.Webservice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Controllers
{
    /// <summary>
    /// Provides methods to add/modify/remove features.
    /// </summary>
    [Authorize]
    [Route("Api/[controller]")]
    public class FeaturesController : Controller
    {
        private readonly FeaturesManager _manager;
        private readonly CmsService _cmsService;

        private bool IsAdministrator => _cmsService.GetUserRole(User.Identity.GetUserIdentity()) == "Administrator";

        public FeaturesController(FeaturesManager manager, CmsService cmsService)
        {
            _manager = manager;
            _cmsService = cmsService;
        }

        /// <summary>
        /// Gets all features.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FeatureResult>), 200)]
        [ProducesResponseType(403)]
        public IActionResult GetAll()
        {
            if (!IsAdministrator)
                return Forbid();

            var features = _manager.GetFeatures(loadGroups: true);
            var results = features.ToList().Select(f => new FeatureResult(f));
            return Ok(results);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        public IActionResult Create([FromBody]FeatureArgs args)
        {
            if (!IsAdministrator)
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _manager.CreateFeature(args);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return StatusCode(409, e.Message);
            }
        }

        /// <summary>
        /// Deletes a feature.
        /// TODO: What happens to descendants? 1) delete whole subtree, 2) move to parent
        /// </summary>
        /// <param name="featureId"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult Delete(int featureId)
        {
            if (!IsAdministrator)
                return Forbid();

            try
            {
                _manager.DeleteFeature(featureId);
                return Ok();
            }
            catch (ResourceNotFoundException e)
            {
                return NotFound(e.Message); // feature does not exist
            }
        }

        [HttpPut("{featureId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        public IActionResult Update(int featureId, [FromBody]FeatureArgs args)
        {
            if (!IsAdministrator)
                return Forbid();

            try
            {
                _manager.UpdateFeature(featureId, args);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return StatusCode(409, e.Message); // new feature name already in use
            }
            catch (ResourceNotFoundException<Feature> e) when ((int)e.Keys.FirstOrDefault() == featureId)
            {
                return NotFound(e.Message); // feature to be updated does not exist
            }
            catch (ResourceNotFoundException<Feature> e)
            {
                return StatusCode(422, e.Message); // referenced parent feature does not exist
            }
        }
    }
}
