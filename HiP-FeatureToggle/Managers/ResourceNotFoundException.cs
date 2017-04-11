using System;
using System.Collections.Generic;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Managers
{
    public class ResourceNotFoundException : Exception
    {
        public IReadOnlyCollection<object> Keys { get; }

        public Type ResourceType { get; }

        /// <param name="key">The key or a collection of keys corresponding to the missing resource(s)</param>
        /// <param name="resourceType">The type of the missing resource(s)</param>
        public ResourceNotFoundException(object key, Type resourceType = null)
            : base(BuildMessage(key, resourceType))
        {
            Keys = (key as IEnumerable<object>)?.ToArray() ?? new[] { key };
            ResourceType = resourceType;
        }

        private static string BuildMessage(object key, Type resourceType)
        {
            var typeString = (resourceType == null) ? "" : $"of type '{resourceType.Name}' ";
            var keys = (key as IEnumerable<object>)?.ToArray() ?? new[] { key };

            switch (keys?.Count() ?? 0)
            {
                case 0: return $"A resource {typeString}cannot be found";
                case 1: return $"The resource '{keys.First()}' {typeString}cannot be found";
                default: return $"The resources [{string.Join(", ", keys)}] {typeString}cannot be found";
            }
        }
    }
}
