using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Utility
{
    /// <summary>
    /// Auth Config
    /// </summary>
    public class AuthConfig
    {
        public string Audience { get; set; }
        public string Authority { get; set; }
    }

    public static class Auth
    {
        /// <summary>
        /// Returns a list of roles of the identity
        /// </summary>
        public static List<Claim> GetUserRoles(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity == null)
                throw new InvalidOperationException("identity not found");

            var role = claimsIdentity.FindAll("role");
            return role.ToList();
        }

        /// <summary>
        /// Checks if the user has Administrator permission
        /// </summary>
        public static bool IsAdministrator(IIdentity identity)
        {
            try
            {
                var roles = identity.GetUserRoles();
                return roles.Any(r => r.Value.Equals("Administrator"));
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
