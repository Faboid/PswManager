using System;

namespace PswManager.Utils;

/// <summary>
/// Provides a way to count all usages of the given <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class RefCount<TValue> {

    /// <summary>
    /// Initializes <see cref="RefCount{TValue}"/> with the given <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// To ensure proper functionality of this class, it's best to avoid using the given <paramref name="value"/> directly. Use <see cref="GetRef"/> to use it.
    /// </remarks>
    /// <param name="value"></param>
    public RefCount(TValue value) {
        this.value = value;
        references = 0;
    }

    /// <summary>
    /// Whether there is any current reference to the <typeparamref name="TValue"/>.
    /// </summary>
    public bool IsInUse => references != 0;

    private readonly TValue value;
    private int references;

    /// <summary>
    /// Retrieves a reference to the stored value to use it.
    /// </summary>
    /// <returns></returns>
    public Ref GetRef() => new(this, value);

    private void Take() {
        references++;
    }
    private void Free() {
        references--;
    }

    /// <summary>
    /// Represents a reference to the value. 
    /// </summary>
    /// <remarks>
    /// It's suggested to use the value only through <see cref="Value"/>, and to call <see cref="Dispose"/> once that value stops being used.
    /// </remarks>
    public struct Ref : IDisposable {

        public readonly TValue Value { get; }

        private readonly RefCount<TValue> refCount;
        private bool isDisposed = false;

        internal Ref(RefCount<TValue> instance, TValue value) {
            Value = value;
            refCount = instance;
            refCount.Take();
        }

        public void Dispose() {
            lock(refCount) {
                if(isDisposed) {
                    throw new ObjectDisposedException(nameof(Ref));
                }

                refCount.Free();
                isDisposed = true;
            }
        }

    }

}
