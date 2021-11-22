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
            AccountBuilder builder = new AccountBuilder(TestsHelper.paths);
            string name = TestsHelper.defaultValues.GetValue(1, DefaultValues.TypeValue.Name);
            (string name, string password, string email) expected = (
                TestsHelper.defaultValues.GetValue(1, DefaultValues.TypeValue.Name),
                TestsHelper.defaultValues.GetValue(1, DefaultValues.TypeValue.Password),
                TestsHelper.defaultValues.GetValue(1, DefaultValues.TypeValue.Email)
                );
            (string name, string password, string email) actual;

            //act
            actual = builder.GetOne(name);
            actual.password = TestsHelper.passCryptoString.Decrypt(actual.password);
            actual.email = TestsHelper.emaCryptoString.Decrypt(actual.email);

            //assert
            Assert.Equal(expected, actual);

        }

    }
}
