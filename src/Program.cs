using Amazon.Lambda.AspNetCoreServer;
using LTExercise.LogAnalysis;
using Microsoft.AspNetCore;

namespace LTExercise
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.
                CreateDefaultBuilder(args).
                UseStartup<Startup>();
        }
    }
    public class LambdaHandler : APIGatewayHttpApiV2ProxyFunction<Program>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return Program.CreateWebHostBuilder(null);
        }
    }
}


