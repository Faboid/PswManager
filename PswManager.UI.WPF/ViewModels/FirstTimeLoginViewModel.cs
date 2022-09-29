using Microsoft.Extensions.Logging;
using PswManager.Core.Services;
using PswManager.UI.WPF.Commands;
using PswManager.UI.WPF.Services;
using System;
using System.Collections;
using System.ComponentModel;

namespace PswManager.UI.WPF.ViewModels;

public class FirstTimeLoginViewModel : ViewModelBase, INotifyDataErrorInfo {

	private readonly ErrorsViewModel _errorsViewModel = new();

	private string _Password = "";
	public string Password {
		get { return _Password; }
		set { 
			SetAndRaise(nameof(Password), ref _Password, value);
			CheckErrors();
		}
	}

	private string _repeatPassword = "";
	public string RepeatPassword {
		get { return _repeatPassword; }
		set { 
			SetAndRaise(nameof(RepeatPassword), ref _repeatPassword, value);
			CheckErrors();
		}
	}

	public bool CanSignUp => !HasErrors && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(RepeatPassword);

	public SignUpAsyncCommand SendCommand { get; }

	public FirstTimeLoginViewModel(CryptoContainerService cryptoContainerService, INotificationService notificationService, ITokenService tokenService, NavigationService<AccountsListingViewModel> navigationToListingViewModel, ILoggerFactory? loggerFactory = null) {
		SendCommand = new SignUpAsyncCommand(() => Password, notificationService, cryptoContainerService, tokenService, navigationToListingViewModel, loggerFactory);
		_errorsViewModel.ErrorsChanged += OnErrorsChanged;
    }

    private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e) {
        ErrorsChanged?.Invoke(this, e);
		OnPropertyChanged(nameof(CanSignUp));
    }

    public bool HasErrors => _errorsViewModel.HasErrors;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName) {
        return _errorsViewModel.GetErrors(propertyName);
    }

    protected override void Dispose(bool disposed) {
        _errorsViewModel.ErrorsChanged -= OnErrorsChanged;
		SendCommand.Dispose();
        base.Dispose(disposed);
    }

	private void CheckErrors() {

        _errorsViewModel.ClearErrors(nameof(Password));
        if(Password.Length < 20) {
            _errorsViewModel.AddError(nameof(Password), "The password must be at least 20 characters long.");
        }

        _errorsViewModel.ClearErrors(nameof(RepeatPassword));
        if(RepeatPassword != Password) {
            _errorsViewModel.AddError(nameof(RepeatPassword), "The password and repeated password must be equal.");
        }

		OnPropertyChanged(nameof(CanSignUp));

    }

}