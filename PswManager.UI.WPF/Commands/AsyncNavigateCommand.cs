using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.ViewModels;
using System;
using System.Threading.Tasks;

namespace PswManager.UI.WPF.Commands;

public class AsyncNavigateCommand<TViewModel, TArgument> : AsyncCommandBase where TViewModel : ViewModelBase {

    private readonly Func<Task<TArgument>> _getArgumentAsync;
    private readonly NavigationService<TViewModel, TArgument> _navigationService;
    private readonly bool _disposeCurrent;

    public AsyncNavigateCommand(Func<Task<TArgument>> lazyArgumentAsync, bool disposeCurrent, NavigationService<TViewModel, TArgument> navigationService) {
        _navigationService = navigationService;
        _disposeCurrent = disposeCurrent;
        _getArgumentAsync = lazyArgumentAsync;
    }

    public AsyncNavigateCommand(Func<Task<TArgument>> lazyArgumentAsync, bool disposeCurrent, NavigationService<TViewModel, TArgument> navigationService, BusyService busyService) : base(busyService) {
        _navigationService = navigationService;
        _disposeCurrent = disposeCurrent;
        _getArgumentAsync = lazyArgumentAsync;
    }

    protected override async Task ExecuteAsync(object? parameter) {
        var argument = await _getArgumentAsync.Invoke();
        _navigationService.Navigate(argument, _disposeCurrent);
    }
}