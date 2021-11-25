using PswManagerLibrary.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Cryptography {
    public class CryptoAccountTests {

        const string passPassword = "ehfgirghriew";
        const string emaPassword = "euh%£@#[YY**§";
        readonly CryptoAccount cryptoAccount = new CryptoAccount(passPassword, emaPassword);

        [Fact]
        public void EncryptAndDecryptCorrectly() {

            //arrange
            string passValue = "hellothere";
            string emaValue = "thsiis#[@this";

            //act
            var encryptedValues = cryptoAccount.Encrypt(passValue, emaValue);
            var decryptedValues = cryptoAccount.Decrypt(encryptedValues.encryptedPassword, encryptedValues.encryptedEmail);

            //assert
            Assert.NotEqual(decryptedValues, encryptedValues);
            Assert.Equal((passValue, emaValue), decryptedValues);

        }

        [Fact]
        public void EncryptToCorrectValue() {

            //arrange
            string passValue = "somerandomvalue";
            string emaValue = "some@colmplex!!value";
            string expectedPassValue = "EtQsQcCTtJmvYk2MBPCmOnZyhbv6EFtLgVMBfGVtlvg=";
            string expectedEmaValue = "5VlDYV/r4pUzhwzmBp3i05Jms1zopvqyVQg+ugaa5I9XIH5KUhhych6f6zokWXRa";

            //act
            var encryptedValues = cryptoAccount.Encrypt(passValue, emaValue);

            //assert
            Assert.Equal((expectedPassValue, expectedEmaValue), encryptedValues);

        }

        [Fact]
        public void DecryptToCorrectValue() {

            //arrange
            string encryptedPassValue = "EtQsQcCTtJmvYk2MBPCmOnZyhbv6EFtLgVMBfGVtlvg=";
            string encryptedEmaValue = "5VlDYV/r4pUzhwzmBp3i05Jms1zopvqyVQg+ugaa5I9XIH5KUhhych6f6zokWXRa";
            string expectedPassValue = "somerandomvalue";
            string expectedEmaValue = "some@colmplex!!value";

            //act
            var decryptedValues = cryptoAccount.Decrypt(encryptedPassValue, encryptedEmaValue);

            //assert
            Assert.Equal((expectedPassValue, expectedEmaValue), decryptedValues);

        }

    }
}
