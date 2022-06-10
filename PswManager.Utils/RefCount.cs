using System;

namespace PswManager.Utils {
    public class RefCount<TValue> {

        public RefCount(TValue value) {
            this.value = value;
            references = 0;
        }

        public bool IsInUse => references != 0;

        private readonly TValue value;
        private int references;

        public Ref GetRef() => new(this, value);

        private void Take() {
            references++;
        }
        private void Free() {
            references--;
        }

        public struct Ref : IDisposable {

            public readonly TValue Value { get; }

            private readonly RefCount<TValue> refCount;
            private bool isDisposed = false;

            public Ref(RefCount<TValue> instance, TValue value) {
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
}
