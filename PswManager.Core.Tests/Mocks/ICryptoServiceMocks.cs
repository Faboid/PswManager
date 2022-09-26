using PswManager.Encryption.Services;

namespace PswManager.Core.Tests.Mocks;
public static class ICryptoServiceMocks {
    
    public static Mock<ICryptoService> GetReverseCryptor() {
        var output = new Mock<ICryptoService>();
        output
            .Setup(x => x.Encrypt(It.IsAny<string>()))
            .Returns<string>(x => new string(x?.Reverse().ToArray() ?? throw new NullReferenceException()));

        output
            .Setup(x => x.Decrypt(It.IsAny<string>()))
            .Returns<string>(x => new string(x?.Reverse().ToArray() ?? throw new NullReferenceException()));

        return output;
    }

    public static Mock<ICryptoService> GetSummingCryptor() {
        var output = new Mock<ICryptoService>();
        output
            .Setup(x => x.Encrypt(It.IsAny<string>()))
            .Returns<string>(x => new string(x?.Select(x => (char)(x + 2)).ToArray() ?? throw new NullReferenceException()));

        output
            .Setup(x => x.Decrypt(It.IsAny<string>()))
            .Returns<string>(x => new string(x?.Select(x => (char)(x - 2)).ToArray() ?? throw new NullReferenceException()));

        return output;
    }

}
