using System;
using System.Threading.Tasks;
using LTExercise;
using LTExercise.LogAnalysis;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace LTExerciseTest;

public class LTExerciseIntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
{
    public LTExerciseIntegrationTest()
    {
    }

    [Fact]
    public async Task GetShould_ReturnCorrectValues()
    {
        var application = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<ILogRepository, LogRepository>();
                    services.AddSingleton<LogService>();
                });
            });

        var client = application.CreateClient();
        var response = await client.GetStringAsync("loganalysis?from=0&to=9999");
        Assert.Equal("[{\"names\":\"  bradley,  carmen,  rob\",\"startTime\":830,\"endTime\":900},{\"names\":\"  bradley,  carmen,  rob\",\"startTime\":1230,\"endTime\":1235}]", response);
    
    }
}