using Microsoft.Extensions.Logging;
using PswManager.Core.AccountModels;
using PswManager.Core.Services;
using PswManager.Database;
using PswManager.Paths;
using PswManager.Utils;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using static PswManager.Core.Password.PasswordStatusChecker;

namespace PswManager.Core.Password;

public class PasswordEditor {

	private readonly ILogger<PasswordEditor> _logger;
	private readonly IDirectoryInfo _dataDirectory;
	private readonly ICryptoAccountServiceFactory _cryptoAccountServiceFactory;
	private readonly PasswordStatusChecker _passwordStatusChecker;
	private readonly BufferHandler _bufferHandler;
	private readonly AccountsHandler _accountsHandler;

	public PasswordEditor(IDirectoryInfoFactory directoryInfoFactory, IFileInfoFactory fileInfoFactory,
						PathsBuilder pathsBuilder, IDataConnection dataConnection,
						IAccountModelFactory currentModelFactory, ICryptoAccountServiceFactory cryptoAccountServiceFactory,
						ILoggerFactory loggerFactory) {
		var bufferDir = directoryInfoFactory.FromDirectoryName(pathsBuilder.GetBufferDataDirectory());
		_dataDirectory = directoryInfoFactory.FromDirectoryName(pathsBuilder.GetDataDirectory());
        _bufferHandler = new(bufferDir, _dataDirectory);
		_accountsHandler = new(dataConnection, currentModelFactory);
		_passwordStatusChecker = new(fileInfoFactory.FromFileName(Path.Combine(pathsBuilder.GetWorkingDirectory(), "DoNotTouch.txt")));
		_cryptoAccountServiceFactory = cryptoAccountServiceFactory;
		_logger = loggerFactory.CreateLogger<PasswordEditor>();
	}

	/// <summary>
	/// Checks if the previous session was interrupted during a password-edit.
	/// If it was, it reverts everything.
	/// </summary>
	/// <returns></returns>
	public async Task StartupCheckup() {
		if(await IsPending()) {
			await _bufferHandler.Restore();
			FreeResources();
		}
	}

	public async Task<PasswordChangeResult> ChangePasswordTo(char[] password) {

		await _passwordStatusChecker.SetStatusTo(PasswordStatus.Starting);
		await _bufferHandler.Backup();
		var newFactory = await GetNewFactory(password);
		var accounts = await _accountsHandler.GetAccounts();
		var rebuiltAccounts = await _accountsHandler.RebuildAccounts(accounts, newFactory);
        using var threads = ThreadsHandler.SetScopedForeground();

		try {

            await _passwordStatusChecker.SetStatusTo(PasswordStatus.Pending);
			await ExecuteUpdate(password, accounts, rebuiltAccounts);
            FreeResources();
			_accountsHandler.UpdateModelFactory(newFactory);
			return PasswordChangeResult.Success;

        } catch(Exception ex) {

			_logger.LogCritical(ex, "An exception has been thrown during the {Pending} phase of changing passwords.", PasswordStatus.Pending);
			_logger.LogWarning("Starting to restore the data to its previous state.");
			await _passwordStatusChecker.SetStatusTo(PasswordStatus.Failed);
			await _bufferHandler.Restore();
			_logger.LogWarning("The data has been restored successfully.");
			FreeResources();
			return PasswordChangeResult.Failure;

		}

	}

	/// <summary>
	/// Deletes the buffer directory and the password status file.
	/// </summary>
	private void FreeResources() {
		_bufferHandler.Free();
		_passwordStatusChecker.Free();
	}

	/// <summary>
	/// Deletes all existing accounts, creates the newly encrypted ones, and creates a new token.
	/// </summary>
	/// <param name="password"></param>
	/// <param name="accountsToDelete"></param>
	/// <param name="newAccounts"></param>
	/// <returns></returns>
	private async Task ExecuteUpdate(char[] password, IExtendedAccountModel[] accountsToDelete, IExtendedAccountModel[] newAccounts) {
		await _accountsHandler.DeleteAccounts(accountsToDelete);
		await _accountsHandler.RecreateAccounts(newAccounts);
		_ = await _cryptoAccountServiceFactory.SignUpAccountAsync(password);
    }

	/// <summary>
	/// Returns whether the password-changing operation is currently in action.
	/// This could either mean it's being currently executed, or that it crashed while trying to execute.
	/// </summary>
	private async Task<bool> IsPending() {
		if(!_bufferHandler.Exists) {
			return false;
		}

        return await (_passwordStatusChecker.GetStatus()) is PasswordStatus.Pending or PasswordStatus.Failed;
	}

	/// <summary>
	/// Builds a new factory without creating a token for it.
	/// </summary>
	/// <param name="password"></param>
	/// <returns></returns>
	private async Task<IAccountModelFactory> GetNewFactory(char[] password) {
        var cryptoAccount = await _cryptoAccountServiceFactory.BuildCryptoAccountService(password);
        return new AccountModelFactory(cryptoAccount);
    }

}

public enum PasswordChangeResult {
	Unknown,
	Success,
	Failure
}