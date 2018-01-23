using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables().Build();

            var port = configurationBuilder.GetValue<string>("Config:Port");

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls(string.IsNullOrEmpty(port) ? "http://*:5000" : $"http://*:{port}")
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
