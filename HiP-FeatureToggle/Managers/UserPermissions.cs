using System;
using System.Linq;
using System.Security.Principal;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Utility;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Managers
{
    public static class UserPermissions
    {
        public static bool IsAllowedToAdminister(IIdentity identity)
        {
            try
            {
                var roles = identity.GetUserRoles();
                return roles.Any(r => r.Value.Equals(Role.Administrator));
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
