using Moq;
using PswManager.Core.Inner;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using Xunit;

namespace PswManager.Core.Tests.Inner {
    public class AccountDeleterTests {

        public AccountDeleterTests() {

            dataDeleter = new();

            dataDeleter
                .Setup(x => x.DeleteAccount(It.IsAny<string>()))
                .Returns<string>(x => new ConnectionResult(!string.IsNullOrWhiteSpace(x)));

            dataDeleter
                .Setup(x => x.DeleteAccountAsync(It.IsAny<string>()))
                .Returns<string>(x => ValueTask.FromResult(new ConnectionResult(!string.IsNullOrWhiteSpace(x))));
        }

        readonly Mock<IDataDeleter> dataDeleter;

        [Theory]
        [InlineData("someName")]
        public void DeleteAccountCallsDBCorrectly(string name) {

            //arrange
            AccountDeleter deleter = new(dataDeleter.Object);

            //act
            var result = deleter.DeleteAccount(name);

            //assert
            Assert.True(result.Success);
            dataDeleter.Verify(x => x.DeleteAccount(It.Is<string>(x => x == name)));
            dataDeleter.VerifyNoOtherCalls();

        }

        [Theory]
        [InlineData("someName")]
        public async Task DeleteAccountCallsDBCorrectlyAsync(string name) {

            //arrange
            AccountDeleter deleter = new(dataDeleter.Object);

            //act
            var result = await deleter.DeleteAccountAsync(name);

            //assert
            Assert.True(result.Success);
            dataDeleter.Verify(x => x.DeleteAccountAsync(It.Is<string>(x => x == name)));
            dataDeleter.VerifyNoOtherCalls();

        }

    }
}
