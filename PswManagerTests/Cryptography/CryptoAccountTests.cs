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
        public void DecryptToCorrectValue() { //if this test fails, it means there's been a non backward-compatible change

            //arrange
            (string encryptedPassValue, string encryptedEmaValue) encryptedValues = (
                "KAB0AGUAcwB0AC4AMQApAAQe25IjOEU2+O9GhAcrHHqKBebY6K2dsWBONDmEupm0OWw55W7fi7ezoyMYNyDMFZ75lXO28s4QDfHTUk/g8YjcI9hxXRvaaOQZMo7bMELZ2ospkgdRcDvEnAX5a5Ccxg==",
                "KAB0AGUAcwB0AC4AMgApANG9Qc9Ss6t4X0nr3/DY51g4ZWNZI9gxOLr6BYN3wAi+jr2KwY8fEMFr2/GP2uQJ/c8t6o9Q2bgIeTkyi+vxYz5tuHKdbKPLxPrJmqHvMl+MY9WnuUJn5iK/4BIypWkzMg=="
                );
            (string expectedPassValue, string expectedEmaValue) expectedValues = ("somerandomvalue", "some@colmplex!!value");

            //act
            var decryptedValues = cryptoAccount.Decrypt(encryptedValues);

            //assert
            Assert.Equal(expectedValues, decryptedValues);

        }

    }
}
