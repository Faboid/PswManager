using System;

namespace PswManager.UI.WPF.Commands;

/// <summary>
/// On execution, executes the given action.
/// </summary>
public class RelayCommand : CommandBase {

    private readonly Action _action;

    public RelayCommand(Action action) {
        _action = action;
    }

    public override void Execute(object? parameter) => _action.Invoke();
}
