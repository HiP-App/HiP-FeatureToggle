using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
		        .UseUrls("http://*:60000")
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
