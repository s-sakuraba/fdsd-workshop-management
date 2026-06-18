using System.IO;
using System.Threading.Tasks;

namespace Fdsd.Application.Abstractions;

public interface IFileStorage
{
    Task SaveAsync(string path, string fileName, Stream content);
    Task DeleteAsync(string path);
    bool Exists(string path);
    Stream OpenRead(string path);
    Task CopyAsync(string sourcePath, string destPath, bool overwrite = false);
    void CreateDirectory(string path);
    bool DirectoryExists(string path);
}