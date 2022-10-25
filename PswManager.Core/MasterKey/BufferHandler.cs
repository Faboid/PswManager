using PswManager.Core.IO;
using System.IO;
using System.Threading.Tasks;

namespace PswManager.Core.MasterKey;

internal class BufferHandler {

    private readonly IDirectoryInfoWrapper _bufferDirectory;
    private readonly IDirectoryInfoWrapper _dataDirectory;

    public BufferHandler(IDirectoryInfoWrapper bufferDirectory, IDirectoryInfoWrapper dataDirectory) {
        _bufferDirectory = bufferDirectory;
        _dataDirectory = dataDirectory;
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

        _dataDirectory.Delete(true);
        await _bufferDirectory.CopyToAsync(_dataDirectory.FullName);
    }

}