using PswManager.Core.IO;
using PswManager.Paths;
using System.IO;
using System.Threading.Tasks;

namespace PswManager.Core.MasterKey;

internal class BufferHandler {

    private readonly IDirectoryInfoWrapper _bufferDirectory;
    private readonly IDirectoryInfoWrapper _dataDirectory;
    private readonly IDirectoryInfoWrapper _logsDirectory;

    public BufferHandler(IDirectoryInfoWrapperFactory directoryInfoWrapperFactory, PathsBuilder pathsBuilder) {
        _bufferDirectory = directoryInfoWrapperFactory.FromDirectoryName(pathsBuilder.GetBufferDataDirectory());
        _dataDirectory = directoryInfoWrapperFactory.FromDirectoryName(pathsBuilder.GetDataDirectory());
        _logsDirectory = directoryInfoWrapperFactory.FromDirectoryName(pathsBuilder.GetLogsDirectory());
    }

    public bool Exists => _bufferDirectory.Exists;

    /// <summary>
    /// Deletes the buffer.
    /// </summary>
    public void Free() => _bufferDirectory.Delete(true);

    /// <summary>
    /// Copies the current <see cref="_dataDirectory"/> to the <see cref="_bufferDirectory"/>.
    /// </summary>
    /// <returns></returns>
    public Task Backup() => _dataDirectory.CopyToAsync(_bufferDirectory.FullName);

    /// <summary>
    /// Deletes the data directory, copies the buffer directory on its place, and then deletes the buffer directory.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public async Task Restore() {
        if(!_bufferDirectory.Exists) {
            throw new DirectoryNotFoundException("Tried to restore the data directory, but the buffer directory does not exist.");
        }

        await BackupLogs();
        _dataDirectory.Delete(true);
        await _bufferDirectory.CopyToAsync(_dataDirectory.FullName);
    }

    private async Task BackupLogs() {
        var path = _logsDirectory.FullName.Replace(_dataDirectory.FullName, _bufferDirectory.FullName);
        await _logsDirectory.CopyToAsync(path);
    }

}