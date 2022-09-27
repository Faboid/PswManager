using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;

namespace PswManager.Core.Tests.Mocks;

public class DataCreatorMock {

    public static Mock<IDataCreator> GetValidatorMock() {
        var dataCreatorMock = new Mock<IDataCreator>();
        dataCreatorMock
            .Setup(x => x.CreateAccountAsync(It.IsAny<AccountModel>()))
            .Returns<AccountModel>(x => Task.FromResult(ValidateValues(x)));

        return dataCreatorMock;
    }

    private static CreatorResponseCode ValidateValues(AccountModel model) {

        if(string.IsNullOrWhiteSpace(model.Name)) return CreatorResponseCode.InvalidName;
        if(string.IsNullOrWhiteSpace(model.Password)) return CreatorResponseCode.MissingPassword;
        if(string.IsNullOrWhiteSpace(model.Email)) return CreatorResponseCode.MissingEmail;

        return CreatorResponseCode.Success;
    }

}
