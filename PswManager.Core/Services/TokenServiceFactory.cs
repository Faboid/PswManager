using PswManager.Paths;
using System.IO.Abstractions;

namespace PswManager.Core.Services;

/// <summary>
/// Provides methods to instantiate a new <see cref="ITokenService"/>.
/// </summary>
public class TokenServiceFactory {

    private readonly IPathsBuilder _pathsHandler;
    private readonly IFileInfoFactory _fileInfoFactory;

    public TokenServiceFactory(IPathsBuilder pathsHandler, IFileInfoFactory fileInfoFactory) {
        _pathsHandler = pathsHandler;
        _fileInfoFactory = fileInfoFactory;
    }

    /// <summary>
    /// Instantiates a new <see cref="ITokenService"/>.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ITokenService CreateTokenService(string token) {

        var path = _pathsHandler.GetTokenPath();
        IFileInfo fileInfo = _fileInfoFactory.FromFileName(path);
        return new TokenService(fileInfo, token);

    }

}