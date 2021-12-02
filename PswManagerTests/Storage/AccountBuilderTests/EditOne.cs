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

            yield return new object[] {
                def.GetValue(1, DefaultValues.TypeValue.Name), "newName1", "newPassword1", "newEmail1",
                ("newName1", "newPassword1", "newEmail1")
            };
            yield return new object[] {
                def.GetValue(2, DefaultValues.TypeValue.Name), null, "randompassword2", null,
                (def.GetValue(2, DefaultValues.TypeValue.Name), "randompassword2", def.GetValue(2, DefaultValues.TypeValue.Email))
            };
            yield return new object[] {
                def.GetValue(1, DefaultValues.TypeValue.Name), null, null, "updatedEmail1",
                (def.GetValue(1, DefaultValues.TypeValue.Name), def.GetValue(1, DefaultValues.TypeValue.Password), "updatedEmail1")
            };
        }

        [Theory]
        [MemberData(nameof(EditOneCorrectlyData))]
        public void EditOneCorrectly(string name, string newName, string newPassword, string newEmail, (string expName, string expPass, string expEma) expected) {

            //arrange
            TestsHelper.SetUpDefault();
            (newPassword, newEmail) = Encrypt(newPassword, newEmail);
            AccountBuilder builder = new AccountBuilder(TestsHelper.Paths);

            //act
            var actual = GenericAct(builder, name, newName, newPassword, newEmail);

            //assert
            Assert.Equal(expected, actual);

        }

        private static (string name, string password, string email) GenericAct(AccountBuilder builder, string name, string newName, string newPassword, string newEmail) {
            builder.EditOne(name, newName, newPassword, newEmail);
            var output = builder.GetOne(newName ?? name);
            (output.password, output.email) = TestsHelper.CryptoAccount.Decrypt(output.password, output.email);

            return output;
        }

        private static (string password, string email) Encrypt(string pass, string ema) {
            if(pass is not null) {
                pass = TestsHelper.CryptoAccount.PassCryptoString.Encrypt(pass);
            }
            if(ema is not null) {
                ema = TestsHelper.CryptoAccount.EmaCryptoString.Encrypt(ema);
            }

            return (pass, ema);
        }

    }
}
