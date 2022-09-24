using System.IO;
using System.IO.Abstractions;

namespace PswManager.Core.Services;

public class TokenServiceFactory {

    private readonly PathsHandler _pathsHandler;

    public TokenServiceFactory(PathsHandler pathsHandler) {
        _pathsHandler = pathsHandler;
    }

    public ITokenService CreateTokenService(string token) {

        var path = _pathsHandler.GetTokenPath();
        IFileInfo fileInfo = new FileInfoWrapper(new FileSystem(), new FileInfo(path));
        return new TokenService(fileInfo, token);

    }

}