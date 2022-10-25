using System.IO.Abstractions;

namespace PswManager.Core.IO;

public class DirectoryInfoWrapperFactory : IDirectoryInfoWrapperFactory {

    private readonly IDirectoryInfoFactory _directoryInfoFactory;

    public DirectoryInfoWrapperFactory(IDirectoryInfoFactory directoryInfoFactory) {
        _directoryInfoFactory = directoryInfoFactory;
    }

    public IDirectoryInfoWrapper FromDirectoryName(string directoryName) {
        return new InternalDirectoryInfoWrapper(_directoryInfoFactory.FromDirectoryName(directoryName));
    }
}
