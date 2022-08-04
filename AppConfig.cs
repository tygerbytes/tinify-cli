using Microsoft.Extensions.Configuration;

namespace TinifyConsole;

public class AppConfig : IAppConfig
{
    public AppConfig()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
}