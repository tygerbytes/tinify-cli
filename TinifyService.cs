using TinifyAPI;

namespace TinifyConsole;

public class TinifyService
{
    public TinifyService(string apiKey)
    {
        Tinify.Key = apiKey;
    }

    public async Task Optimize(string fileName)
    {
        await RunOperationAsync(nameof(Optimize), fileName, async () =>
        {
            var source = await Tinify.FromFile(fileName);
            return source;
        });
    }

    public async Task Resize(int width, int height, string fileName)
    {
        await RunOperationAsync(nameof(Resize), fileName, async () =>
        {
            var source = await Tinify.FromFile(fileName);
            var resized = source.Resize(new
            {
                method = "fit",
                width = width,
                height = height
            });
            return resized;
        });
    }

    private async Task RunOperationAsync(string operationName, string fileName, Func<Task<Source>> opFunc)
    {
        if (!IsImageFile(fileName))
        {
            Console.WriteLine($"Not an image file: '{fileName}'");
            return;
        }
        
        var outputDir = GetOutputDir();

        var message = $"{operationName}: '{fileName}'. ";

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
    
    private static string GetOutputDir()
    {
        const string processedDirName = "processed";
        if (!Directory.Exists(processedDirName))
        {
            Directory.CreateDirectory(processedDirName);
        }

        return processedDirName;
    }
}