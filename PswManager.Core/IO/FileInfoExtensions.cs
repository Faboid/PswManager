using System.IO.Abstractions;
using System.Threading.Tasks;

namespace PswManager.Core.IO;

public static class FileInfoExtensions {

    /// <summary>
    /// Copies an existing file to a new file asynchronously.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task CopyToAsync(this IFileInfo info, string path) {
        using var reader = info.OpenRead();
        using var writer = info.FileSystem.File.Create(path);
        await reader.CopyToAsync(writer);
    }

}
