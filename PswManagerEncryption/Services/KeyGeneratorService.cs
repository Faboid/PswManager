using PswManagerEncryption.Cryptography;
using PswManagerEncryption.Random;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

[assembly: InternalsVisibleTo("PswManagerTests")]
namespace PswManagerEncryption.Services {
    public class KeyGeneratorService : IDisposable {

        /// <summary>
        /// Instantiates a <see cref="KeyGeneratorService"/> with a fixed salt. 
        /// As long as the given <paramref name="masterKey"/> is the same, the generated keys will be consistent.
        /// </summary>
        /// <param name="masterKey"></param>
        public KeyGeneratorService(char[] masterKey) : this(new byte[] { 64, 12, 35, 44, 21, 34, 43 }, masterKey) { }

        /// <summary>
        /// Instantiates a <see cref="KeyGeneratorService"/> with a custom <paramref name="salt"/>.
        /// As long as <paramref name="salt"/> (and <paramref name="masterKey"/>) is the same, the generated keys will be consistent.
        /// </summary>
        /// <param name="salt"></param>
        /// <param name="masterKey"></param>
        public KeyGeneratorService(byte[] salt, char[] masterKey) : this(salt, masterKey, 1000000){ }

        /// <summary>
        /// Instantiates a <see cref="KeyGeneratorService"/> with a custom <paramref name="salt"/>.
        /// As long as <paramref name="salt"/>, <paramref name="masterKey"/>, and <paramref name="iterations"/> are the same, the generated keys will be consistent.
        /// </summary>
        /// <param name="salt"></param>
        /// <param name="masterKey"></param>
        /// <param name="iterations"></param>
        internal KeyGeneratorService(byte[] salt, char[] masterKey, int iterations) {
            this.salt = salt;
            this.masterKey = new Key(masterKey);
            this.iterations = iterations;
            keys = GenerateKeys();
        }

        private readonly byte[] salt;
        private readonly Key masterKey;
        private readonly int iterations;
        private readonly IEnumerable<Key> keys;

        /// <summary>
        /// Generates a finite amount of keys.
        /// <br/>As long as the given <see cref="salt"/> and <see cref="masterKey"/> are the same, 
        /// the generated series will be the same across instances.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IEnumerable<Key> GenerateKeys(int num) {
            return keys.Take(num);
        }

        //this is set to private to protect from infinite loops
        private IEnumerable<Key> GenerateKeys() {

            byte[] bytes = Encoding.Unicode.GetBytes(masterKey.Get());
            var rfc = new Rfc2898DeriveBytes(bytes, salt, iterations);

            var random = new SaltRandom(64, 168, bytes.Sum(x => x / (bytes.Length / 2)));
            while(true) {
                yield return new Key(rfc.GetBytes(random.Next()));
            }
        }

        public void Dispose() {
            masterKey.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
