using System.Windows;

namespace PswManager.UI.WPF.Commands;

/// <summary>
/// Minimizes the window when executed.
/// </summary>
public class MinimizeCommand : CommandBase {

    private readonly Window _window;

    public MinimizeCommand(Window window) {
        _window = window;
    }

    public override void Execute(object? parameter) {
        _window.WindowState = WindowState.Minimized;
    }
}
