using Xunit;
using PswManagerLibrary.Cryptography;

namespace PswManagerTests.Cryptography {
    public class CryptoStringTests {

        [Fact]
        public void EncryptAndDecryptCorrectly() {

            //arrange
            string password = "rithwydif£ghg\"ytirhs%";
            CryptoString crypto = new CryptoString(password);
            string valueToTest = "test/ng this r@ndom value!";

            //act
            var encryptedValue = crypto.Encrypt(valueToTest);


            var decryptedValue = crypto.Decrypt(encryptedValue);

            //assert
            Assert.NotEqual(valueToTest, encryptedValue);
            Assert.Equal(valueToTest, decryptedValue);

        }

        [Fact]
        public void EncryptToCorrectValue() {

            //arrange
            string password = "rithwy*£^^ghg\"ytirhs%";
            CryptoString crypto = new CryptoString(password);
            string valueToTest = "test/ng this r@ndom valu?!";

            string expectedResult = "558JNWxBo76STB6WMoWc/VLp7adpDqNZf+qipe+OYAN8Q1etp52Y2lVCtCChUsfFDWXKjYI10FfWgxzKAzTYZw==";

            //act
            var encryptedValue = crypto.Encrypt(valueToTest);

            //assert
            Assert.Equal(expectedResult, encryptedValue);

        }

        [Fact]
        public void DecryptToCorrectValue() {

            //arrange
            string password = "rithwy*£^^ghg\"ytirhs%";
            CryptoString crypto = new CryptoString(password);
            string valueToTest = "558JNWxBo76STB6WMoWc/VLp7adpDqNZf+qipe+OYAN8Q1etp52Y2lVCtCChUsfFDWXKjYI10FfWgxzKAzTYZw==";

            string expectedResult = "test/ng this r@ndom valu?!";

            //act
            var encryptedValue = crypto.Decrypt(valueToTest);

            //assert
            Assert.Equal(expectedResult, encryptedValue);

        }

    }
}
