using System.IO.Abstractions;
namespace PswManager.Core.IO;

/// <summary>
/// <inheritdoc cref="IDirectoryInfoFactory"/>
/// </summary>
public interface IDirectoryInfoWrapperFactory {
    IDirectoryInfoWrapper FromDirectoryName(string directoryName);
}
