namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Services
{
    /// <summary>
    /// Dummy service that fakes authorization.
    /// </summary>
    public class CmsService
    {
        public string GetUserRole(string identity) => "Administrator";
    }
}
