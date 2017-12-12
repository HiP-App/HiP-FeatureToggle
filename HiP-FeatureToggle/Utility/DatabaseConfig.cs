using PaderbornUniversity.SILab.Hip.Webservice;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Utility
{
    public class DatabaseConfig : PostgresDatabaseConfig
    {
        /// <summary>
        /// Name of the database to use.
        /// Default value: "hipFeatureToggleDb"
        /// </summary>
        public override string Name { get; set; } = "hipFeatureToggleDb";
    }
}
