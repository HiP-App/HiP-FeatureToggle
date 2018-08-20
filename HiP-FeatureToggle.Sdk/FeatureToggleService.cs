using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle
{
    /// <summary>
    /// A service that can be used with ASP.NET Core dependency injection.
    /// Usage: In ConfigureServices():
    /// <code>
    /// services.Configure&lt;FeatureToggleConfig&gt;(Configuration.GetSection("Endpoints"));
    /// services.AddSingleton&lt;FeatureToggleService&gt;();
    /// </code>
    /// </summary>
    public class FeatureToggleService
    {

        private readonly FeatureToggleConfig _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FeatureToggleService(IOptions<FeatureToggleConfig> config, ILogger<FeatureToggleService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _config = config.Value;
            _httpContextAccessor = httpContextAccessor;

            if (string.IsNullOrWhiteSpace(config.Value.FeatureToggleHost))
                logger.LogWarning($"{nameof(FeatureToggleConfig.FeatureToggleHost)} is not configured correctly!");
        }

        public FeaturesClient Features => new FeaturesClient(_config.FeatureToggleHost)
        {
            Authorization = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
        };

        public FeatureGroupsClient FeatureGroups => new FeatureGroupsClient(_config.FeatureToggleHost)
        {
            Authorization = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
        };

    }
}

