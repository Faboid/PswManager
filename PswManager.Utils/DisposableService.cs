using System.Threading;

namespace PswManager.Utils;

/// <summary>
/// Provides a method to handle one-time only operations that must not be repeated.
/// </summary>
public class DisposableService {
    private int _isDisposed = 0;

    /// <summary>
    /// Returns true in the first call, then always false. Is thread-safe.
    /// </summary>
    /// <returns></returns>
    public bool StartDisposal() {
        return Interlocked.Exchange(ref _isDisposed, 1) == 0;
    }

}