using Moq;
using PswManagerLibrary.Cryptography;
using PswManagerLibrary.UIConnection;

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

        public static IUserInput GetDefaultAutoInput() {
            const string genericAnswer = "DefaultComputerAnswer";
            var userInput = new Mock<IUserInput>();
            userInput.Setup(x => x.SendMessage(It.IsAny<string>()));
            userInput.Setup(x => x.RequestAnswer(It.IsAny<string>())).Returns(genericAnswer);
            userInput.Setup(x => x.RequestAnswer()).Returns(genericAnswer);
            userInput.Setup(x => x.YesOrNo(It.IsAny<string>())).Returns(true);

            return userInput.Object;
        }

    }
}
