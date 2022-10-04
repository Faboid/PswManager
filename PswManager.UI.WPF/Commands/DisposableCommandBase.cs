using PswManager.Utils;
using System;

namespace PswManager.UI.WPF.Commands;

public abstract class DisposableCommandBase : CommandBase, IDisposable {
    private readonly DisposableService _disposableService = new();
    
    protected virtual void Dispose(bool disposing) { }

    public void Dispose() {
        if(_disposableService.StartDisposal()) {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}