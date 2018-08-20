namespace PaderbornUniversity.SILab.Hip.FeatureToggle
{
    /// <summary>
    /// Configuration properties for clients using the FeatureToggle SDK.
    /// </summary>
    public sealed class FeatureToggleConfig
    {
        /// <summary>
        /// URL pointing to a running instance of the FeatureToggle service.
        /// Example: "https://docker-hip.cs.upb.de/develop/feature-toggle"
        /// </summary>
        public string FeatureToggleHost { get; set; }
    }
}
