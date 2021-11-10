using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Storage {
    public class Token : IToken {

        readonly CryptoString passCryptoString;
        readonly CryptoString emaCryptoString;
        readonly IPaths paths;

        public Token(CryptoString passCryptoString, CryptoString emaCryptoString, IPaths paths) {
            this.passCryptoString = passCryptoString;
            this.emaCryptoString = emaCryptoString;
            this.paths = paths;
        }

        public void Set(string passToken, string emaToken) {

            var tokens = new[] { passCryptoString.Encrypt(passToken), emaCryptoString.Encrypt(emaToken) };

            File.WriteAllLines(paths.TokenFilePath, tokens);
        }

        public (string passToken, string emaToken) Get() {
            var savedToken = File.ReadAllLines(paths.TokenFilePath);

            //decrypt saved tokens
            return (passCryptoString.Decrypt(savedToken[0]), emaCryptoString.Decrypt(savedToken[1]));
        }

        public bool Confront(string passToken, string emaToken) {
            if(File.Exists(paths.TokenFilePath) is false) {
                return false;
            }

            //put the tokens in an array to facilitate comparison
            var tokens = new[] { passToken, emaToken };

            //get encrypted tokens
            var savedToken = File.ReadAllLines(paths.TokenFilePath);

            //decrypt saved tokens
            var savedTokens = new[] { passCryptoString.Decrypt(savedToken[0]), emaCryptoString.Decrypt(savedToken[1]) };

            //compare given and decrypted tokens
            return savedTokens == tokens;

        }

    }
}
