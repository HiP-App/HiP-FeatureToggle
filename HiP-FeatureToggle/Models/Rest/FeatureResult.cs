using PaderbornUniversity.SILab.Hip.FeatureToggle.Models.Entity;
using System.Collections.Generic;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Models.Rest
{
    public class FeatureResult
    {
        public int Id { get; }

        public string Name { get; }

        public int Parent { get; }

        public IReadOnlyList<int> GroupsWhereEnabled { get; }

        public FeatureResult(Feature feature)
        {
            Id = feature.Id;
            Name = feature.Name;
            Parent = feature.ParentId;
            GroupsWhereEnabled = feature.GroupsWhereEnabled?.Select(g => g.GroupId).ToList();
        }
    }
}
