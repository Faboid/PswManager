using PswManager.Encryption.Services;
using PswManager.Core.Cryptography;
using Moq;
using PswManager.Core;
using PswManager.Database.Models;

namespace PswManager.ConsoleUI.Tests.Commands.Helper; 
internal static class MockedObjects {

    /// <summary>
    /// Gets an ICryptoString that doesn't encrypt—it will return the same values that are given to it.
    /// </summary>
    /// <returns></returns>
    public static ICryptoService GetEmptyCryptoString() {
        var cryptoString = new Mock<ICryptoService>();
        cryptoString.Setup(x => x.Encrypt(It.IsAny<string>())).Returns<string>(x => x);
        cryptoString.Setup(x => x.Decrypt(It.IsAny<string>())).Returns<string>(x => x);

        return cryptoString.Object;
    }

    /// <summary>
    /// Gets an ICryptoAccount that doesn't encrypt—it will return the same values that are given to it.
    /// </summary>
    /// <returns></returns>
    public static ICryptoAccount GetEmptyCryptoAccount() {
        var cryptoString = GetEmptyCryptoString();

        var cryptoAccount = new Mock<ICryptoAccount>();
        cryptoAccount.Setup(x => x.GetPassCryptoService()).Returns(cryptoString);
        cryptoAccount.Setup(x => x.GetEmaCryptoService()).Returns(cryptoString);
        cryptoAccount.Setup(x => x.Encrypt(It.IsAny<(string, string)>())).Returns<(string, string)>(x => x);
        cryptoAccount.Setup(x => x.Decrypt(It.IsAny<(string, string)>())).Returns<(string, string)>(x => x);
        cryptoAccount.Setup(x => x.Encrypt(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((pass, ema) => (pass, ema));
        cryptoAccount.Setup(x => x.Decrypt(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((pass, ema) => (pass, ema));
        cryptoAccount.Setup(x => x.Encrypt(It.IsAny<AccountModel>())).Returns<AccountModel>(x => x);
        cryptoAccount.Setup(x => x.Decrypt(It.IsAny<AccountModel>())).Returns<AccountModel>(x => x);

        return cryptoAccount.Object;
    }

    /// <summary>
    /// Gets an IUserInput that will mimic an user input. 
    /// <br/>It will always return "yes" to YesOrNo() and "DefaultComputerAnswer" to any query that asks for a response.
    /// </summary>
    /// <returns></returns>
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
