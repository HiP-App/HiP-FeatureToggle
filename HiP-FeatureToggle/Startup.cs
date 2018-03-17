using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSwag.AspNetCore;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Data;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Managers;
using PaderbornUniversity.SILab.Hip.Webservice;
using PaderbornUniversity.SILab.Hip.Webservice.Logging;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // load both the appsettings and the appsettings.Development /
            // appsettings.Production files into the Configuration attribute
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = configurationBuilder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Inject a configuration with the properties from AppConfig that
            // match the given Configuration (which was loaded in the constructor).
            services.Configure<PostgresDatabaseConfig>(Configuration.GetSection("Database"))
                    .Configure<AuthConfig>(Configuration.GetSection("Auth"))
                    .Configure<LoggingConfig>(Configuration.GetSection("HiPLoggerConfig"))

            var serviceProvider = services.BuildServiceProvider(); // allows us to actually get the configured services
            var authConfig = serviceProvider.GetService<IOptions<AuthConfig>>();
            var dbConfig = serviceProvider.GetService<IOptions<PostgresDatabaseConfig>>();

            // Configure authentication
            services
                .AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = authConfig.Value.Audience;
                    options.Authority = authConfig.Value.Authority;
                });

            // Configure authorization
            var domain = authConfig.Value.Authority;
            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:featuretoggle",
                    policy => policy.Requirements.Add(new HasScopeRequirement("read:featuretoggle", domain)));
                options.AddPolicy("write:featuretoggle",
                    policy => policy.Requirements.Add(new HasScopeRequirement("write:featuretoggle", domain)));
                options.AddPolicy("write:cms",
                    policy => policy.Requirements.Add(new HasScopeRequirement("write:cms", domain)));
            });

            services.AddCors();
            services.AddDbContext<ToggleDbContext>(options => options.UseNpgsql(dbConfig.Value.ConnectionString));
            services.AddMvc();

            // Add managers
            services.AddTransient<FeatureGroupsManager>();
            services.AddTransient<FeaturesManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, ToggleDbContext dbContext, IOptions<LoggingConfig> loggingConfig)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"))
                         .AddHipLogger(loggingConfig.Value); 

            if (env.IsDevelopment())
                loggerFactory.AddDebug();

            app.UseRequestSchemeFixer();

            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin());

            app.UseAuthentication();
            app.UseMvc();
            app.UseSwaggerUiHip();

            // Run migrations
            dbContext.Database.Migrate();
            ToggleDbInitializer.Initialize(dbContext);

            loggerFactory.CreateLogger("ApplicationStartup").LogInformation("FeatureToggle started successfully");
        }
    }
}
