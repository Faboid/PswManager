using PswManagerEncryption.Cryptography;
using PswManagerEncryption.Services;
using System.Linq;
using System.Reflection;
using Xunit;

namespace PswManagerTests.Encryption {
    public class CryptoServiceTests {

        [Fact]
        public void FreePasswordCorrectly() {

            //arrange
            char[] password = "tihruehgyrtiwioghjruy".ToCharArray();
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            var field = typeof(CryptoService).GetField("key", flags);
            char[] storedPass = new char[password.Length];
            bool assigned = false;

            //act
            using(var crypto = new CryptoService(password)) {
                storedPass = (field.GetValue(crypto) as Key).Get();
                assigned = storedPass!.Any(x => x != 0);
            }

            //assert
            Assert.True(password.All(x => x == default));
            Assert.True(assigned);
            Assert.True(storedPass!.All(x => x == default));

        }

        [Fact]
        public void EncryptAndDecryptCorrectly() {

            //arrange
            string password = "rithwydif£ghg\"ytirhs%";
            using var crypto = new CryptoService(password.ToCharArray());
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
            using var crypto = new CryptoService(password.ToCharArray());
            string valueToTest = "test/ng this r@ndom valu?!";

            //act
            var encryptedValue = crypto.Encrypt(valueToTest);

            //assert
            Assert.NotEqual(valueToTest, encryptedValue);

        }

        [Fact]
        public void DecryptToCorrectValue() { //if this test breaks, it means there's no backward compatibility

            //arrange
            string password = "rithwy*£^^ghg\"ytirhs%";
            using var crypto = new CryptoService(password.ToCharArray());
            string valueToTest = "3n7NRlaJpI7TNB3DwaNxg/XXvbJmghh/OFhge2xl" +
                "8oUMA2G/auk2yfJLITEtjPyAc401DUsfky6kldjzy2wRXnvv1ATZK6Bcx" +
                "6erHeoxoK/c5D8N13ERdUAiGzEGirQrdAOK921GQh/R2CcBzU5hqzwhdq" +
                "f/Ecd8YO0rGvysOkYy9PAF4LeX+ngXEf6hLcDTiAGHyA4ydyq9ujVHUC9" +
                "fmhJQpJXuJHp9pxhqSI8XQoslOUT5wYEYMHUeZwE5wT9zpo6XCiLTfhQh" +
                "5j1xRF/4gnK/N2xqqhicx/VqYyw35Iu96sDbnfPhFHwpiLqmGgbI4YYOg" +
                "Cuu+SlZcgYHJyjKK7ocSCuL2u3FLMLuPw==";

            string expectedResult = "test/ng this r@ndom valu?!";

            //act
            var encryptedValue = crypto.Decrypt(valueToTest);

            //assert
            Assert.Equal(expectedResult, encryptedValue);

        }

    }
}
