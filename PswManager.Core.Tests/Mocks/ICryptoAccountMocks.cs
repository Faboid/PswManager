using PswManager.Core.Services;

namespace PswManager.Core.Tests.Mocks;

public class ICryptoAccountMocks {

    public static ICryptoAccountService GetReversedAndSummingCryptor() {
        var passCryptoService = ICryptoServiceMocks.GetSummingCryptor().Object;
        var emaCryptoService = ICryptoServiceMocks.GetReverseCryptor().Object;
        return new CryptoAccountService(emaCryptoService, passCryptoService);
    }

}