using Moq;
using PswManager.Core.Inner;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using Xunit;

namespace PswManager.Core.Tests.Inner {
    public class AccountDeleterTests {

        public AccountDeleterTests() {

            dataDeleterMock = new();

            dataDeleterMock
                .Setup(x => x.DeleteAccount(It.IsAny<string>()))
                .Returns<string>(x => new ConnectionResult(!string.IsNullOrWhiteSpace(x)));

            dataDeleterMock
                .Setup(x => x.DeleteAccountAsync(It.IsAny<string>()))
                .Returns<string>(x => ValueTask.FromResult(new ConnectionResult(!string.IsNullOrWhiteSpace(x))));
        }

        readonly Mock<IDataDeleter> dataDeleterMock;

        [Theory]
        [InlineData("someName")]
        public void DeleteAccountCallsDBCorrectly(string name) {

            //arrange
            AccountDeleter deleter = new(dataDeleterMock.Object);

            //act
            var result = deleter.DeleteAccount(name);

            //assert
            Assert.True(result.Success);
            dataDeleterMock.Verify(x => x.DeleteAccount(It.Is<string>(x => x == name)));
            dataDeleterMock.VerifyNoOtherCalls();

        }

        [Theory]
        [InlineData("someName")]
        public async Task DeleteAccountAsyncCallsDBCorrectly(string name) {

            //arrange
            AccountDeleter deleter = new(dataDeleterMock.Object);

            //act
            var result = await deleter.DeleteAccountAsync(name);

            //assert
            Assert.True(result.Success);
            dataDeleterMock.Verify(x => x.DeleteAccountAsync(It.Is<string>(x => x == name)));
            dataDeleterMock.VerifyNoOtherCalls();

        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public async Task NoCallsIfInvalidName(string name) {

            //arrange
            AccountDeleter creator = new(dataDeleterMock.Object);

            //act
            var result = creator.DeleteAccount(name);
            var resultAsync = await creator.DeleteAccountAsync(name);

            //assert
            Assert.False(result.Success);
            Assert.False(resultAsync.Success);
            dataDeleterMock.VerifyNoOtherCalls();

        }

    }
}
