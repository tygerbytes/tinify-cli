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
        
        [Option(Description = "The search string to match against the names of files in the local directory")]
        public string FilePattern { get; }

        public async Task<int> RunAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
        {
            Tinify.Key = ApiKey;
            
            const string processedDirName = "processed";
            if (!Directory.Exists(processedDirName))
            {
                Directory.CreateDirectory(processedDirName);
            }
            
            var files = Directory.EnumerateFiles("./", FilePattern, SearchOption.TopDirectoryOnly);

            foreach (string path in files)
            {
                if (
                    !
                    (path.EndsWith(".png")
                    || path.EndsWith(".jpg")
                    || path.EndsWith(".webp"))
                    )
                {
                    // Doesn't appear to be an image file
                    continue;
                }

                var fileName = Path.GetFileName(path);
                
                Console.WriteLine($"Uploading '{fileName}'");
                
                var source = Tinify.FromFile(fileName);
            
                Console.WriteLine($"Downloading compressed file to '{processedDirName}/{fileName}'");
                await source.ToFile($"{processedDirName}/{fileName}"); 
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