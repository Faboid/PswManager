using Moq;
using PswManagerLibrary.Cryptography;

namespace PswManagerTests.Commands.Helper {
    internal static class MockedObjects {

        /// <summary>
        /// Gets an ICryptoAccount that doesn't encrypt—it will return the same values that are given to it.
        /// </summary>
        /// <returns></returns>
        public static ICryptoAccount GetEmptyCryptoAccount() {
            var cryptoAccount = new Mock<ICryptoAccount>();
            cryptoAccount.Setup(x => x.Encrypt(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((pass, ema) => (pass, ema));
            cryptoAccount.Setup(x => x.Decrypt(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((pass, ema) => (pass, ema));

            return cryptoAccount.Object;
        }

    }
}
