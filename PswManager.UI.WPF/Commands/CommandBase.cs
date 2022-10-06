using System;
using System.Windows.Input;

namespace PswManager.UI.WPF.Commands;

/// <summary>
/// Represents a generic command's framework.
/// </summary>
public abstract class CommandBase : ICommand {

    public event EventHandler? CanExecuteChanged;

    public virtual bool CanExecute(object? parameter) {
        return true;
    }

    public abstract void Execute(object? parameter);

    protected void OnCanExecuteChanged() {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

}
