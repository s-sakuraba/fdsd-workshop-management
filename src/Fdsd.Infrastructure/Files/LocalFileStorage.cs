using System.IO;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;

namespace Fdsd.Infrastructure.Files;

public class LocalFileStorage : IFileStorage
{
    public Task SaveAsync(string path, string fileName, Stream content)
    {
        var fullPath = Path.Combine(path, fileName);
        var dir = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(dir) && dir != null)
            Directory.CreateDirectory(dir);

        using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        return content.CopyToAsync(fileStream);
    }

    public Task DeleteAsync(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
        return Task.CompletedTask;
    }

    public bool Exists(string path) => File.Exists(path);

    public Stream OpenRead(string path) => new FileStream(path, FileMode.Open, FileAccess.Read);

    public Task CopyAsync(string sourcePath, string destPath, bool overwrite = false)
    {
        var destDir = Path.GetDirectoryName(destPath);
        if (!Directory.Exists(destDir) && destDir != null)
            Directory.CreateDirectory(destDir);

        File.Copy(sourcePath, destPath, overwrite);
        return Task.CompletedTask;
    }

    public void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    public bool DirectoryExists(string path) => Directory.Exists(path);
}