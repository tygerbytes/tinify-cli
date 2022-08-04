using Microsoft.Extensions.Configuration;

namespace TinifyConsole;

public interface IAppConfig
{
    IConfiguration Configuration { get; }
}