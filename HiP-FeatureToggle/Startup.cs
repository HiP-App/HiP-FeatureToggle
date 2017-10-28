using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using PaderbornUniversity.SILab.Hip.Webservice;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Data;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Managers;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Utility;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle
{
    public class Startup
    {
        private const string Version = "v1";
        private const string Name = "HiP Feature Toggle API";

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
            services.Configure<AppConfig>(Configuration);
            services.Configure<AuthConfig>(Configuration.GetSection("Auth"));

            var serviceProvider = services.BuildServiceProvider(); // allows us to actually get the configured services
            var authConfig = serviceProvider.GetService<IOptions<AuthConfig>>();

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
            
            // Add Cross Origin Requests 
            services.AddCors();

            services.AddDbContext<ToggleDbContext>(
                options => options.UseNpgsql(AppConfig.BuildConnectionString(Configuration))
            );

            // Register the Swagger generator
            services.AddSwaggerGen(c =>
            {
                // Define a Swagger document
                c.SwaggerDoc("v1", new Info() { Title = Name, Version = Version });
                c.OperationFilter<CustomSwaggerOperationFilter>();
                c.IncludeXmlComments(Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml"));
            });

            // Add framework services.
            services.AddMvc();

            // Add managers
            services.AddTransient<FeatureGroupsManager>();
            services.AddTransient<FeaturesManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IOptions<AppConfig> appConfig,
            ToggleDbContext dbContext,
            IOptions<AuthConfig> authConfig)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            if (env.IsDevelopment())
                loggerFactory.AddDebug();

            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin());

            app.UseAuthentication();
            app.UseMvc();

            // Swagger / Swashbuckle configuration:

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
            });

            // Configure SwaggerUI endpoint
            app.UseSwaggerUI(c =>
            {
                // TODO: Only a hack, if HiP-Swagger is running, SwaggerUI can be disabled for Production
                c.SwaggerEndpoint((env.IsDevelopment() ? "/swagger" : "..") +
                                  "/" + Version + "/swagger.json", Name + Version);
            });

            // Run migrations
            dbContext.Database.Migrate();
            ToggleDbInitializer.Initialize(dbContext);
        }
    }
}
