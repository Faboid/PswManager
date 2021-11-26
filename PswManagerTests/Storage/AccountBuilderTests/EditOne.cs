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
    public class EditOne {

        public static IEnumerable<object[]> EditOneCorrectlyData() {
            var def = TestsHelper.DefaultValues;

            yield return new string[] {
                def.GetValue(1, DefaultValues.TypeValue.Name), "newName1", "newPassword1", "newEmail1",
                "newName1 newPassword1 newEmail1"
            };
            yield return new string[] {
                def.GetValue(2, DefaultValues.TypeValue.Name), null, "randompassword2", null,
                $"{def.GetValue(2, DefaultValues.TypeValue.Name)} randompassword2 {def.GetValue(2, DefaultValues.TypeValue.Email)}"
            };
            yield return new string[] {
                def.GetValue(1, DefaultValues.TypeValue.Name), null, null, "updatedEmail1",
                $"{def.GetValue(1, DefaultValues.TypeValue.Name)} {def.GetValue(1, DefaultValues.TypeValue.Password)} updatedEmail1"
            };
        }

        //todo - fix this mess of a test
        [Theory]
        [MemberData(nameof(EditOneCorrectlyData))]
        public void EditOneCorrectly(string name, string newName, string newPassword, string newEmail, string expectedstring) {

            //arrange
            TestsHelper.SetUpDefault();
            if(newPassword is not null) {
                newPassword = TestsHelper.CryptoAccount.PassCryptoString.Encrypt(newPassword);
            }
            if(newEmail is not null) {
                newEmail = TestsHelper.CryptoAccount.EmaCryptoString.Encrypt(newEmail);
            }

            AccountBuilder builder = new AccountBuilder(TestsHelper.Paths);

            var splitstrings = expectedstring.Split(' ');
            (string name, string password, string email) expected = (splitstrings[0], splitstrings[1], splitstrings[2]);
            (string name, string password, string email) actual;

            //act
            builder.EditOne(name, newName, newPassword, newEmail);
            actual = builder.GetOne(newName ?? name);
            (actual.password, actual.email) = TestsHelper.CryptoAccount.Decrypt(actual.password, actual.email);

            //assert
            Assert.Equal(expected, actual);

        }

    }
}
