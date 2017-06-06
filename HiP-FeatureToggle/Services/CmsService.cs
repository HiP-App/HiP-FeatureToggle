using System.Security.Claims;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Services
{
    /// <summary>
    /// Dummy service that fakes authorization.
    /// </summary>
    public class CmsService
    {
        public string GetUserRole(ClaimsPrincipal user) => "Administrator";
    }
}
