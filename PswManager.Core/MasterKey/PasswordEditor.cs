using Microsoft.Extensions.Logging;
using PswManager.Core.AccountModels;
using PswManager.Core.IO;
using PswManager.Core.Services;
using PswManager.Database;
using PswManager.Paths;
using PswManager.Utils;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static PswManager.Core.MasterKey.PasswordStatusChecker;

[assembly:InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace PswManager.Core.MasterKey;

public class PasswordEditor {

	private readonly ILogger<PasswordEditor>? _logger;
	private readonly ICryptoAccountServiceFactory _cryptoAccountServiceFactory;
	private readonly IPasswordStatusChecker _passwordStatusChecker;
	private readonly IBufferHandler _bufferHandler;
	private readonly AccountsHandler _accountsHandler;

	public PasswordEditor(IDirectoryInfoWrapperFactory directoryInfoFactory, IFileInfoFactory fileInfoFactory,
						PathsBuilder pathsBuilder, IDataConnection dataConnection,
						IAccountModelFactory currentModelFactory, ICryptoAccountServiceFactory cryptoAccountServiceFactory,
						ILoggerFactory? loggerFactory) {
		_bufferHandler = new BufferHandler(directoryInfoFactory, pathsBuilder);
		_accountsHandler = new(dataConnection, currentModelFactory);
		_passwordStatusChecker = new PasswordStatusChecker(fileInfoFactory.FromFileName(Path.Combine(pathsBuilder.GetWorkingDirectory(), "DoNotTouch.txt")));
		_cryptoAccountServiceFactory = cryptoAccountServiceFactory;
		_logger = loggerFactory?.CreateLogger<PasswordEditor>();
	}

	/// <summary>
	/// Constructor used for testing.
	/// </summary>
	internal PasswordEditor(IBufferHandler bufferHandler, IPasswordStatusChecker passwordStatusChecker, 
							IDataConnection dataConnection, IAccountModelFactory currentModelFactory, 
							ICryptoAccountServiceFactory cryptoAccountServiceFactory) {
		_bufferHandler = bufferHandler;
		_passwordStatusChecker = passwordStatusChecker;
		_accountsHandler = new(dataConnection, currentModelFactory);
		_cryptoAccountServiceFactory = cryptoAccountServiceFactory;
	}

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
	/// Changes the master key by decrypting and re-encrypting all accounts with the new password.
	/// If you call this, ensure to run <see cref="StartupCheckup"/> on the next application boot to resolve any possible crashes/errors.
	/// </summary>
	/// <param name="password"></param>
	/// <returns></returns>
	public async Task<PasswordChangeResult> ChangePasswordTo(char[] password) {

		_logger?.LogInformation("Beginning the password-changing operation.");
		await _passwordStatusChecker.SetStatusTo(PasswordStatus.Starting);
		await _bufferHandler.Backup();
		_logger?.LogInformation("Completed backup of the current data.");
		var newFactory = await GetNewFactory(password);
		var accounts = await _accountsHandler.GetAccounts();
		var rebuiltAccounts = await _accountsHandler.RebuildAccounts(accounts, newFactory);
		_logger?.LogInformation("The accounts have been rebuilt with the new password.");
		using var threads = ThreadsHandler.SetScopedForeground();

		try {

			_logger?.LogInformation("Completed the {Starting} phase. Beginning the {Pending} phase.", PasswordStatus.Starting, PasswordStatus.Pending);
			await _passwordStatusChecker.SetStatusTo(PasswordStatus.Pending);
			await ExecuteUpdate(password, accounts, rebuiltAccounts);
			FreeResources();
			_accountsHandler.UpdateModelFactory(newFactory);
			_logger?.LogInformation("The password has been changed successfully.");
			return PasswordChangeResult.Success;

		} catch(Exception ex) {

			_logger?.LogCritical(ex, "An exception has been thrown during the {Pending} phase of changing passwords.", PasswordStatus.Pending);
			_logger?.LogWarning("Starting to restore the data to its previous state.");
			await _passwordStatusChecker.SetStatusTo(PasswordStatus.Failed);
			await _bufferHandler.Restore();
			_logger?.LogWarning("The data has been restored successfully.");
			FreeResources();
			_logger?.LogInformation("The backups have been freed.");
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
		_logger?.LogInformation("Beginning deleting, recreating the accounts, and setting up new token.");
		await _accountsHandler.DeleteAccounts(accountsToDelete);
		_logger?.LogInformation("Deleted the accounts successfully.");
		await _accountsHandler.RecreateAccounts(newAccounts);
		_logger?.LogInformation("Recreated the accounts successfully.");
		_ = await _cryptoAccountServiceFactory.SignUpAccountAsync(password);
		_logger?.LogInformation("Set up new token successfully.");
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