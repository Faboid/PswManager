using PswManager.Core.Services;

namespace PswManager.UI.WPF.Services;

/// <summary>
/// Provides a way to provide the main password to the account pipeline before it's set up.
/// </summary>
public class CryptoContainerService {

	public ICryptoAccountService? CryptoAccountService { get; set; }

}