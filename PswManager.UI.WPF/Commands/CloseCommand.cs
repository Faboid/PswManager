using System.Windows;

namespace PswManager.UI.WPF.Commands;

/// <summary>
/// Shuts down the window.
/// </summary>
public class CloseCommand : CommandBase {

    private readonly Window _window;

    public CloseCommand(Window window) {
        _window = window;
    }

    public override void Execute(object? parameter) {
        _window.Close();
    }

}
