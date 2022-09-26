using PswManager.Core.AccountModels;
using PswManager.Database.Models;

namespace PswManager.Core.Tests.Asserts;
internal class AccountModelAsserts {

    public static bool AssertEqual(AccountModel expected, AccountModel actual) {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.Password);
        Assert.Equal(expected.Email, actual.Email);
        return true;
    }

    public static bool AssertEqual(IAccountModel expected, AccountModel actual) {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.Password);
        Assert.Equal(expected.Email, actual.Email);
        return true;
    }

    public static bool AssertEqual(AccountModel expected, IAccountModel actual) {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.Password);
        Assert.Equal(expected.Email, actual.Email);
        return true;
    }

    public static bool AssertEqual(IAccountModel expected, IAccountModel actual) {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.Password);
        Assert.Equal(expected.Email, actual.Email);
        return true;
    }

    public static bool AssertEqual(IAccountModel expected, IAccount actual) {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.EncryptedPassword);
        Assert.Equal(expected.Email, actual.EncryptedEmail);
        return true;
    }

}
