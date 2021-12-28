using PswManagerLibrary.Cryptography;
using Xunit;

namespace PswManagerTests.Cryptography {
    public class CryptoAccountTests {

        const string passPassword = "ehfgirghriew";
        const string emaPassword = "euh%£@#[YY**§";
        readonly CryptoAccount cryptoAccount = new CryptoAccount(passPassword, emaPassword);

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
        public void EncryptToCorrectValue() {

            //arrange
            (string passValue, string emaValue) values = ("somerandomvalue", "some@colmplex!!value");
            (string expectedPassValue, string expectedEmaValue) expected = (
                "EtQsQcCTtJmvYk2MBPCmOnZyhbv6EFtLgVMBfGVtlvg=", 
                "5VlDYV/r4pUzhwzmBp3i05Jms1zopvqyVQg+ugaa5I9XIH5KUhhych6f6zokWXRa"
                );

            //act
            var encryptedValues = cryptoAccount.Encrypt(values);

            //assert
            Assert.Equal(expected, encryptedValues);

        }

        [Fact]
        public void DecryptToCorrectValue() {

            //arrange
            (string encryptedPassValue, string encryptedEmaValue) encryptedValues = (
                "EtQsQcCTtJmvYk2MBPCmOnZyhbv6EFtLgVMBfGVtlvg=",
                "5VlDYV/r4pUzhwzmBp3i05Jms1zopvqyVQg+ugaa5I9XIH5KUhhych6f6zokWXRa"
                );
            (string expectedPassValue, string expectedEmaValue) expectedValues = ("somerandomvalue", "some@colmplex!!value");

            //act
            var decryptedValues = cryptoAccount.Decrypt(encryptedValues);

            //assert
            Assert.Equal(expectedValues, decryptedValues);

        }

    }
}
