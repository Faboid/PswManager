using PswManager.Extensions;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace PswManager.Core.IO;

public static class DirectoryInfoExtensions {

    /// <summary>
    /// Copies to another directory. All subdirectories and files will be copied to it with their relative paths.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="path"></param>
    public static void CopyTo(this IDirectoryInfo info, string path) {
        info.FileSystem.Directory.CreateDirectory(path);

        //create all directories
        info
            .EnumerateDirectories("*", SearchOption.AllDirectories)
            .Select(x => x.FullName.Replace(info.FullName, path))
            .ForEach(x =>  info.FileSystem.Directory.CreateDirectory(x));

        //copy all files
        info
            .EnumerateFiles("*", SearchOption.AllDirectories)
            .Select(x => (File: x, NewPath: x.FullName.Replace(info.FullName, path)))
            .ForEach(x => x.File.CopyTo(x.NewPath));
    }

    /// <summary>
    /// Copies to another directory asynchronously. All subdirectories and files will be copied to it with their relative paths.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task CopyToAsync(this IDirectoryInfo info, string path) {
        info.FileSystem.Directory.CreateDirectory(path);

        //create all directories
        info
            .EnumerateDirectories("*", SearchOption.AllDirectories)
            .Select(x => x.FullName.Replace(info.FullName, path))
            .ForEach(x => info.FileSystem.Directory.CreateDirectory(x));

        //copy all files
        var tasks = info.EnumerateFiles("*", SearchOption.AllDirectories)
            .Select(x => (File: x, NewFile: x.FullName.Replace(info.FullName, path)))
            .Select(x => x.File.CopyToAsync(x.NewFile))
            .ToArray();

        await Task.WhenAll(tasks);
    }

}
