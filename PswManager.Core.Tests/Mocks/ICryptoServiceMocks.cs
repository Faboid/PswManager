using Moq;
using PswManager.Encryption.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManager.Core.Tests.Mocks {
    public static class ICryptoServiceMocks {
        
        //decryption methods aren't yet required. Might add later

        public static Mock<ICryptoService> GetReverseEncryptor() {
            var output = new Mock<ICryptoService>();
            output
                .Setup(x => x.Encrypt(It.IsAny<string>()))
                .Returns<string>(x => new string(x?.Reverse().ToArray() ?? throw new NullReferenceException()));
            return output;
        }

        public static Mock<ICryptoService> GetStringToOneCharEncryptor() {
            var output = new Mock<ICryptoService>();
            output
                .Setup(x => x.Encrypt(It.IsAny<string>()))
                .Returns<string>(x => x?.Aggregate((x, y) => (char)(x + y)).ToString() ?? throw new NullReferenceException());
            return output;
        }

    }
}
