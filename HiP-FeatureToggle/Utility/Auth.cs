using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Utility
{
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
}
}
