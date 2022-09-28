using System.IO;
using System.IO.Abstractions;

namespace PswManager.Core.Services;

public class TokenServiceFactory {

    private readonly PathsHandler _pathsHandler;
    private readonly IFileInfoFactory _fileInfoFactory;

    public TokenServiceFactory(PathsHandler pathsHandler, IFileInfoFactory fileInfoFactory) {
        _pathsHandler = pathsHandler;
        _fileInfoFactory = fileInfoFactory;
    }

    public ITokenService CreateTokenService(string token) {

        var path = _pathsHandler.GetTokenPath();
        IFileInfo fileInfo = _fileInfoFactory.FromFileName(path);
        return new TokenService(fileInfo, token);

    }

}