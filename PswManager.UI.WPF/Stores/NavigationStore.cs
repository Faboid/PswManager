using PswManager.UI.WPF.ViewModels;
using System;

namespace PswManager.UI.WPF.Stores;

public class NavigationStore {

    private ViewModelBase? _currentViewModel;
    public ViewModelBase? CurrentViewModel {
        get => _currentViewModel;
        set {
            _currentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    private void OnCurrentViewModelChanged() {
        CurrentViewModelChanged?.Invoke();
    }

    public event Action? CurrentViewModelChanged;

}
