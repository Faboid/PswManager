using PswManager.Core.AccountModels;
using PswManager.Core.Services;
using PswManager.Core.Validators;

namespace PswManager.Core.Tests.ValidatorsTests;

public class AccountValidatorTests {

    private readonly IAccountValidator _sut = new AccountValidator();

    public static IEnumerable<object[]> TestData() {
        static object[] NewTestCase(IAccountModel model, AccountValid expected) => new object[] { model, expected };

        yield return NewTestCase(NewAccountModel("ValidName", "ValidPassword", "ValidEmail@Email.com"), AccountValid.Valid);

        yield return NewTestCase(NewAccountModel(null!, "ValidPassword", "ValidEmail@Email.com"), AccountValid.NameEmptyOrNull);
        yield return NewTestCase(NewAccountModel("    ", "ValidPassword", "ValidEmail@Email.com"), AccountValid.NameEmptyOrNull);
        yield return NewTestCase(NewAccountModel("", "ValidPassword", "ValidEmail@Email.com"), AccountValid.NameEmptyOrNull);

        yield return NewTestCase(NewAccountModel("ValidName", null!, "ValidEmail@Email.com"), AccountValid.PasswordEmptyOrNull);
        yield return NewTestCase(NewAccountModel("ValidName", "", "ValidEmail@Email.com"), AccountValid.PasswordEmptyOrNull);
        yield return NewTestCase(NewAccountModel("ValidName", "   ", "ValidEmail@Email.com"), AccountValid.PasswordEmptyOrNull);

        yield return NewTestCase(NewAccountModel("ValidName", "ValidPassword", null!), AccountValid.EmailEmptyOrNull);
        yield return NewTestCase(NewAccountModel("ValidName", "ValidPassword", ""), AccountValid.EmailEmptyOrNull);
        yield return NewTestCase(NewAccountModel("ValidName", "ValidPassword", "    "), AccountValid.EmailEmptyOrNull);

        //for consistency
        yield return NewTestCase(NewAccountModel("", "", ""), AccountValid.NameEmptyOrNull);
        yield return NewTestCase(NewAccountModel("ValidName", "", null!), AccountValid.PasswordEmptyOrNull);

    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void IsValidReturnsCorrectResult(IAccountModel model, AccountValid expected) {

        Assert.Equal(expected, _sut.IsAccountValid(model));

    }

    private static IAccountModel NewAccountModel(string name, string password, string email) {
        return new DecryptedAccount(name, password, email, Mock.Of<ICryptoAccountService>());
    }

}