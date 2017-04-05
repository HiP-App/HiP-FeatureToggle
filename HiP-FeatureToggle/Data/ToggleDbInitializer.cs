using PaderbornUniversity.SILab.Hip.FeatureToggle.Models.Entity;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Data
{
    /// <summary>
    /// Populates an empty database with the default feature group.
    /// </summary>
    public static class ToggleDbInitializer
    {
        public static void Initialize(ToggleDbContext db)
        {
            if (db.FeatureGroups.Any())
                return; // DB is already seeded

            var defaultGroup = new FeatureGroup
            {
                Name = "Default"
            };

            db.FeatureGroups.Add(defaultGroup);
            db.SaveChanges();
        }
    }
}
