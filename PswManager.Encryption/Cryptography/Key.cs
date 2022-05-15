using System.Runtime.InteropServices;
using System.Text;

namespace PswManager.Encryption.Cryptography {
    public class Key : IDisposable {

        public Key(byte[] password) : this(Encoding.Unicode.GetChars(password)) {
            for(int i = 0; i < password.Length; i++) { 
                password[i] = 0;
            }
        }

        public Key(char[] password) {
            key = new char[password.Length];
            gcHandle = GCHandle.Alloc(key, GCHandleType.Pinned);
            password.CopyTo(key, 0);
            Clear(password);
        }

        private readonly char[] key;
        private GCHandle gcHandle;

        public char[] Get() {
            return key;
        }

        private static void Clear(char[] arr) {
            for(int i = 0; i < arr.Length; i++) {
                arr[i] = default;
            }
        }

        public void Dispose() {
            Clear(key);
            gcHandle.Free();
            GC.SuppressFinalize(this);
        }
    }
}
