using PswManager.Core.AccountModels;
using PswManager.Database.Models;

namespace PswManager.Core.Tests.Asserts;
public class AccountModelAsserts {

    public static bool AssertEqual(AccountModel expected, AccountModel actual) {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.Password);
        Assert.Equal(expected.Email, actual.Email);
        return true;
    }

    public static bool AssertEqual(IExtendedAccountModel expected, AccountModel actual) {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.Password);
        Assert.Equal(expected.Email, actual.Email);
        return true;
    }

    public static bool AssertEqual(AccountModel expected, IExtendedAccountModel actual) {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.Password);
        Assert.Equal(expected.Email, actual.Email);
        return true;
    }

    public static bool AssertEqual(IExtendedAccountModel expected, IExtendedAccountModel actual) {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.Password);
        Assert.Equal(expected.Email, actual.Email);
        return true;
    }

    public static bool AssertEqual(IExtendedAccountModel expected, IAccount actual) {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.EncryptedPassword);
        Assert.Equal(expected.Email, actual.EncryptedEmail);
        return true;
    }

    public static bool AssertEqual(AccountModel expected, IAccount actual) {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.EncryptedPassword);
        Assert.Equal(expected.Email, actual.EncryptedEmail);
        return true;
    }

}
