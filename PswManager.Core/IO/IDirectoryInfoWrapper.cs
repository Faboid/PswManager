using System.IO.Abstractions;
using System.Threading.Tasks;

namespace PswManager.Core.IO;

/// <summary>
/// <inheritdoc cref="IDirectoryInfo"/>.
/// <br/>
/// Interface wrapper to enhance mocking capability in tests.
/// </summary>
public interface IDirectoryInfoWrapper : IDirectoryInfo {

    /// <summary>
    /// <inheritdoc cref="DirectoryInfoExtensions.CopyTo(IDirectoryInfo, string)"/>
    /// </summary>
    /// <param name="path"></param>
    public void CopyTo(string path);

    /// <summary>
    /// <inheritdoc cref="DirectoryInfoExtensions.CopyToAsync(IDirectoryInfo, string)"/>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Task CopyToAsync(string path);

}