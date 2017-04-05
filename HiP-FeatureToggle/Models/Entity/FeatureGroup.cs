using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Models.Entity
{
    public class FeatureGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<User> Members { get; set; }

        /// <summary>
        /// The features that are enabled in this feature group.
        /// The opposite direction of this many-to-many relation is <see cref="Feature.GroupsWhereEnabled"/>.
        /// </summary>
        public IList<FeatureToFeatureGroupMapping> EnabledFeatures { get; set; }
    }
}
