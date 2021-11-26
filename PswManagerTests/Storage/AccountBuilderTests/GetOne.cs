using PswManagerLibrary.Storage;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Storage.AccountBuilderTests {

    [Collection("TestHelperCollection")]
    public class GetOne {

        [Fact]
        public void GetOneShouldReturn() {

            //arrange
            TestsHelper.SetUpDefault();
            AccountBuilder builder = new AccountBuilder(TestsHelper.Paths);
            string name = TestsHelper.DefaultValues.GetValue(1, DefaultValues.TypeValue.Name);
            (string name, string password, string email) expected = (
                TestsHelper.DefaultValues.GetValue(1, DefaultValues.TypeValue.Name),
                TestsHelper.DefaultValues.GetValue(1, DefaultValues.TypeValue.Password),
                TestsHelper.DefaultValues.GetValue(1, DefaultValues.TypeValue.Email)
                );
            (string name, string password, string email) actual;

            //act
            actual = builder.GetOne(name);
            (actual.password, actual.email) = TestsHelper.CryptoAccount.Decrypt(actual.password, actual.email);

            //assert
            Assert.Equal(expected, actual);

        }

    }
}
