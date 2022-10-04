using PswManager.Core.AccountModels;
using PswManager.Database.Models;

namespace PswManager.Core.Tests.Asserts;
public class AccountModelAsserts {

    public static bool AssertEqual(IReadOnlyAccountModel expected, IReadOnlyAccountModel actual) {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.Password);
        Assert.Equal(expected.Email, actual.Email);
        return true;
    }

}
