using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using TinifyAPI;

namespace TinifyConsole
{
    [ValidateRequiredParameters]
    [Command(Name = "tinifyconsole", Description = "Client for Tinify")]
    [HelpOption("-h")]
    public class Program
    {
        private readonly IAppConfig _appConfig;

        public static async Task<int> Main(string[] args)
        {
            var appConfig = new AppConfig();
            
            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(ConfigureServices(appConfig));

            var returnCode = await app.ExecuteAsync(args);
            return returnCode;
        }

        public Program(IAppConfig appConfig)
        {
            _appConfig = appConfig;
        }
        
        [Option(Description = "Tinify API Key")]
        public string ApiKey { get; }
        
        [Option(Description = "Reduce the file size of the image")]
        public bool Optimize { get; }
        
        [Option(Description = "Resize the image")]
        public bool Resize { get; }
        
        [Option(Description = "New width of image (works with --resize)")]
        public int Width { get; }
        
        [Option(Description = "New height of image (works with --resize)")]
        public int Height { get; }
        
        [Option(Description = "The search string to match against the names of files in the local directory")]
        public string FilePattern { get; }
        

        public async Task<int> RunAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
        {
            var tinify = new TinifyService(ApiKey);
            
            var files = Directory.EnumerateFiles("./", FilePattern, SearchOption.TopDirectoryOnly);

            foreach (var path in files)
            {
                var fileName = Path.GetFileName(path);
                
                if (Resize)
                {
                    await tinify.Resize(width: Width, height: Height, fileName: fileName);
                }
                else 
                {
                    // Default to optimizing the image
                    await tinify.Optimize(fileName);
                }
                
            }

            await Task.CompletedTask;
            return 0;
        }
        
        private static ServiceProvider ConfigureServices(IAppConfig appConfig)
        {
            var services = new ServiceCollection()
                .AddSingleton<IAppConfig>(appConfig)
                .BuildServiceProvider();

            return services;
        }
        
        /// <summary>
        /// Called by CommandLineApplication
        /// </summary>
        /// <param name="app"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
        {
            return await RunAsync(app, cancellationToken);
        }
    }
} 