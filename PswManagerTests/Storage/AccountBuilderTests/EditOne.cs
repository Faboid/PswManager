using PswManagerLibrary.Exceptions;
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
        public void EditOneCorrectly_WithName(string name, string newName, string newPassword, string newEmail, (string expName, string expPass, string expEma) expected) {

            //arrange
            TestsHelper.SetUpDefault();
            (newPassword, newEmail) = Encrypt(newPassword, newEmail);
            AccountBuilder builder = new AccountBuilder(TestsHelper.Paths);

            //act
            builder.EditOne(name, newName, newPassword, newEmail);
            var actual = builder.GetOne(newName ?? name);
            actual = Decrypt(actual);

            //assert
            Assert.Equal(expected, actual);

        }

        [Theory]
        [MemberData(nameof(EditOneCorrectlyData))]
        public void EditOneCorrectly_WithPosition(string name, string newName, string newPassword, string newEmail, (string expName, string expPass, string expEma) expected) {

            //arrange
            TestsHelper.SetUpDefault();
            (newPassword, newEmail) = Encrypt(newPassword, newEmail);
            AccountBuilder builder = new AccountBuilder(TestsHelper.Paths);
            int position = builder.Search(name) ?? throw new InexistentAccountException("error in provided test values");

            //act
            builder.EditOne(position, newName, newPassword, newEmail);
            var actual = builder.GetOne(position);
            actual = Decrypt(actual);

            //assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void EditOneFailure_InexistentName() {

            //arrange
            AccountBuilder builder = new AccountBuilder(TestsHelper.Paths);
            string inexistantName = "erighty";
            string arg = "empty";

            //act

            //assert
            Assert.Throws<InexistentAccountException>(() => builder.EditOne(inexistantName, arg, arg, arg));

        }

        [Fact]
        public void EditOneFailure_InexistentPosition() {

            //arrange
            AccountBuilder builder = new AccountBuilder(TestsHelper.Paths);
            int inexistentPos = 100;
            string arg = "empty";

            //act

            //assert
            Assert.Throws<InexistentAccountException>(() => builder.EditOne(inexistentPos, arg, arg, arg));

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

        private static (string name, string password, string email) Decrypt((string name, string pass, string ema) values) {
            if(values.pass is not null) {
                values.pass = TestsHelper.CryptoAccount.PassCryptoString.Decrypt(values.pass);
            }
            if(values.ema is not null) {
                values.ema = TestsHelper.CryptoAccount.EmaCryptoString.Decrypt(values.ema);
            }

            return values;
        }

    }
}
