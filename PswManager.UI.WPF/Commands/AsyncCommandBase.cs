using PswManager.UI.WPF.Services;
using System.Threading.Tasks;

namespace PswManager.UI.WPF.Commands;

/// <summary>
/// Represents a linkable asynchronous command.
/// </summary>
public abstract class AsyncCommandBase : AsyncLinkableCommandBase {

    public AsyncCommandBase() { }
    public AsyncCommandBase(BusyService busyService) : base(busyService) { }

    public override async Task ExecuteLinkedAsync(object? parameter) {
        await ExecuteAsync(parameter);
    }

    protected abstract Task ExecuteAsync(object? parameter);

}
