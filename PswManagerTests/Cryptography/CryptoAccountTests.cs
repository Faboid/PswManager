using PswManagerEncryption.Services;
using PswManagerLibrary.Cryptography;
using Xunit;

namespace PswManagerTests.Cryptography {
    public class CryptoAccountTests {

        public CryptoAccountTests() {
            var passCryptoService = new CryptoService(passPassword, "test.1");
            var emaCryptoService = new CryptoService(emaPassword, "test.2");
            cryptoAccount = new CryptoAccount(passCryptoService, emaCryptoService);
        }

        readonly char[] passPassword = "ehfgirghriew".ToCharArray();
        readonly char[] emaPassword = "euh%£@#[YY**§".ToCharArray();
        readonly CryptoAccount cryptoAccount;

        [Fact]
        public void EncryptAndDecryptCorrectly() {

            //arrange
            (string passValue, string emaValue) values = ("hellothere", "thsiis#[@this");

            //act
            var encryptedValues = cryptoAccount.Encrypt(values);
            var decryptedValues = cryptoAccount.Decrypt(encryptedValues);

            //assert
            Assert.NotEqual(decryptedValues, encryptedValues);
            Assert.Equal(values, decryptedValues);

        }

        [Fact]
        public void EncryptToCipherText() {

            //arrange
            (string passValue, string emaValue) = ("somerandomvalue", "some@colmplex!!value");

            //act
            var (encryptedPassword, encryptedEmail) = cryptoAccount.Encrypt((passValue, emaValue));

            //assert
            Assert.NotEqual(passValue, encryptedPassword);
            Assert.NotEqual(emaValue, encryptedEmail);

        }

        [Fact]
        public void DecryptToCorrectValue() {

            //arrange
            (string encryptedPassValue, string encryptedEmaValue) encryptedValues = (
                "KAB0AGUAcwB0AC4AMQApAKwj9b+YJLx8V2R4EtQDsBfI78d9cOTksBYKJTBvBd1UQ5/0nCA5CpW4R4T4/+N159zpba/tPH/qNdBP0Wh7lJ3jFPB2XwWSSZ/D4WA=",
                "KAB0AGUAcwB0AC4AMgApAJ9sQm+IrJzIt5dPktkKIHbxvW+d0WfhTx8eaqk8nqcsUgSdeQKHBcLTO6ltOQrNVzpF2TzoijztOsiOmNf7Uvmf0paUBDPUK9QsZVKQ3gFMotr9KFpIj/G4gtDunBLyhPy5xI1pTv08+ZJf"
                );
            (string expectedPassValue, string expectedEmaValue) expectedValues = ("somerandomvalue", "some@colmplex!!value");

            //act
            var decryptedValues = cryptoAccount.Decrypt(encryptedValues);

            //assert
            Assert.Equal(expectedValues, decryptedValues);

        }

    }
}
