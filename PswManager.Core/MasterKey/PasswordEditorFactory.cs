using Microsoft.Extensions.Logging;
using PswManager.Core.AccountModels;
using PswManager.Core.IO;
using PswManager.Core.Services;
using PswManager.Database;
using PswManager.Paths;
using System.IO.Abstractions;
using System.IO;
using System.Threading.Tasks;
using static PswManager.Core.MasterKey.PasswordStatusChecker;

namespace PswManager.Core.MasterKey;

public class PasswordEditorFactory {

    private readonly IDataConnection _dataConnection;
    private readonly ICryptoAccountServiceFactory _cryptoAccountServiceFactory;
    private readonly IPasswordStatusChecker _passwordStatusChecker;
    private readonly IBufferHandler _bufferHandler;
    private readonly ILoggerFactory? _loggerFactory;
    private readonly ILogger<PasswordEditorFactory>? _logger;

    public PasswordEditorFactory(IDirectoryInfoWrapperFactory directoryInfoFactory, IFileInfoFactory fileInfoFactory,
                    IPathsBuilder pathsBuilder, IDataConnection dataConnection, ICryptoAccountServiceFactory cryptoAccountServiceFactory,
                    ILoggerFactory? loggerFactory = null) {
        _bufferHandler = new BufferHandler(directoryInfoFactory, pathsBuilder);
        _passwordStatusChecker = new PasswordStatusChecker(fileInfoFactory.FromFileName(Path.Combine(pathsBuilder.GetWorkingDirectory(), "DoNotTouch.txt")));
        _cryptoAccountServiceFactory = cryptoAccountServiceFactory;
        _dataConnection = dataConnection;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory?.CreateLogger<PasswordEditorFactory>();
    }

    /// <summary>
    /// Testing constructor.
    /// </summary>
    internal PasswordEditorFactory(IBufferHandler bufferHandler,
                        IPasswordStatusChecker passwordStatusChecker,
                        ICryptoAccountServiceFactory cryptoAccountServiceFactory,
                        IDataConnection dataConnection) {
        _bufferHandler = bufferHandler;
        _passwordStatusChecker = passwordStatusChecker;
        _cryptoAccountServiceFactory = cryptoAccountServiceFactory;
        _dataConnection = dataConnection;
    }

    public IPasswordEditor BuildPasswordEditor(IAccountModelFactory currentFactory) 
        => new PasswordEditor(_bufferHandler, _passwordStatusChecker, new AccountsHandler(_dataConnection, currentFactory), _cryptoAccountServiceFactory);


    /// <summary>
    /// Checks if the previous session was interrupted during a password-edit.
    /// If it was, it reverts everything.
    /// </summary>
    /// <returns></returns>
    public async Task StartupCheckup() {
        if(await IsPending()) {
            _logger?.LogWarning("The startup check has found out that the last session's password-changing operation has been interrupted. Trying to restore...");
            await _bufferHandler.Restore();
            FreeResources();
            _logger?.LogWarning("Everything has been restored successfully.");
        }
    }

    /// <summary>
    /// Returns whether the password-changing operation is currently in action.
    /// This could either mean it's being currently executed, or that it crashed while trying to execute.
    /// </summary>
    private async Task<bool> IsPending() {
        if(!_bufferHandler.Exists) {
            return false;
        }

        return await _passwordStatusChecker.GetStatus() is PasswordStatus.Pending or PasswordStatus.Failed;
    }

    /// <summary>
    /// Deletes the buffer directory and the password status file.
    /// </summary>
    private void FreeResources() {
        _bufferHandler.Free();
        _passwordStatusChecker.Free();
    }


}