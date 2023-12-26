using TinifyAPI;

namespace TinifyConsole;

public class TinifyService
{
    private readonly bool overwrite;

    public TinifyService(string apiKey, bool overwrite)
    {
        Tinify.Key = apiKey;
        this.overwrite = overwrite;
    }

    public async Task Optimize(string fileName)
    {
        await RunOperationAsync(nameof(Optimize), fileName, async () =>
        {
            var source = await Tinify.FromFile(fileName);
            return source;
        },
        overwrite: this.overwrite);
    }

    public async Task Resize(int width, int height, string path)
    {
        await RunOperationAsync(nameof(Resize), path, async () =>
        {
            var source = await Tinify.FromFile(path);
            var resized = source.Resize(new
            {
                method = "fit",
                width = width,
                height = height
            });
            return resized;
        },
        overwrite: this.overwrite);
    }

    private async Task RunOperationAsync(string operationName, string path, Func<Task<Source>> opFunc, bool overwrite = false)
    {
        var fileName = Path.GetFileName(path);
        if (!IsImageFile(fileName))
        {
            Console.WriteLine($"Not an image file: '{path}'");
            return;
        }

        var dir = Path.GetDirectoryName(path);
        var outputDir = GetOutputDir(dir, overwrite);

        var message = $"{operationName}: '{path}'. ";

        try
        {
            var source = await opFunc();
            await source.ToFile(Path.Join(outputDir, fileName)); 
        }
        catch (Exception e)
        {
            Console.WriteLine($"❌FAIL: {message}\n\t{e}");
            return;
        }
            
        Console.WriteLine($"✅DONE: {message}");
    }
    
    private static bool IsImageFile(string path)
    {
        return
            path.EndsWith(".jpg")
            || path.EndsWith(".png")
            || path.EndsWith(".webp");
    }
    
    private static string GetOutputDir(string? relativePath = null, bool overwrite = false)
    {
        var processedDirName = "";
        if (!overwrite)
        {
            processedDirName = "processed";
        }

        relativePath ??= "./";

        var processedDir = Path.Combine(relativePath, processedDirName);

        if (!Directory.Exists(processedDir))
        {
            Directory.CreateDirectory(processedDir);
        }

        return processedDir;
    }
}