using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Managers
{
    public class ResourceNotFoundException : Exception
    {
        public IReadOnlyCollection<object> Keys { get; }

        public Type ResourceType { get; }

        /// <param name="keys">The keys corresponding to the missing resources</param>
        /// <param name="resourceType">The type of the missing resources</param>
        public ResourceNotFoundException(IEnumerable<object> keys, Type resourceType = null)
            : base(BuildMessage(keys, resourceType))
        {
            Keys = keys.ToList();
            ResourceType = resourceType;
        }

        /// <param name="key">The key corresponding to the missing resource</param>
        /// <param name="resourceType">The type of the missing resource</param>
        public ResourceNotFoundException(object key, Type resourceType = null) : this(new[] { key }, resourceType)
        {
        }

        private static string BuildMessage(IEnumerable<object> keys, Type resourceType)
        {
            var typeString = (resourceType == null) ? "" : $"of type '{resourceType.Name}' ";

            switch (keys?.Count() ?? 0)
            {
                case 0: return $"A resource {typeString}cannot be found";
                case 1: return $"The resource '{keys.First()}' {typeString}cannot be found";
                default: return $"The resources [{string.Join(", ", keys)}] {typeString}cannot be found";
            }
        }
    }
}
