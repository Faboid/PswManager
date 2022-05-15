using PswManager.Encryption.Cryptography;
using PswManager.Encryption.Services;
using System.Linq;
using System.Reflection;
using Xunit;

namespace PswManager.Tests.Encryption {
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
            using var crypto = new CryptoService(new Key(password.ToCharArray()), "test.1");
            string valueToTest = "test/ng this r@ndom value!";
            
            //act
            var encryptedValue = crypto.Encrypt(valueToTest);


            var decryptedValue = crypto.Decrypt(encryptedValue);

            //assert
            Assert.NotEqual(valueToTest, encryptedValue);
            Assert.Equal(valueToTest, decryptedValue);

        }

        [Fact]
        public void EncryptToCipherText() {

            //arrange
            string password = "rithwy*£^^ghg\"ytirhs%";
            using var crypto = new CryptoService(password.ToCharArray(), "test.1");
            string valueToTest = "test/ng this r@ndom valu?!";

            //act
            var encryptedValue = crypto.Encrypt(valueToTest);

            //assert
            Assert.NotEqual(valueToTest, encryptedValue);

        }

        [Theory]
        [InlineData("KAB0AGUAcwB0AC4AMQApAFZJM8Q9gfiOIvSXkkDmB5Miq" +
            "Ef0Br2vhv+p3MfgBA0qlA9j9oXQCkt3zaomurjW+LSOOidtLUQweW" +
            "GP4+fSly3W9H6YUGtfOFnsCygXWVnISwRXow2B5otX8EVNSBIJ/Ge" +
            "W/blYu+mUaDy65Hv8h3Qm/g3UK1omjw==")] //tests "test.1" version
        [InlineData("KAB0AGUAcwB0AC4AMgApAB2x7kjH6a9srledPT7yw4yNv" +
            "j2q5BhFsdgFbEj8FzL7Ddeww8OM01XkJuj66KYqnn8rdJ+j6uXpXh" +
            "TCjjJEN1v1OM/gN/nA39kfrW7Tdr2rtCefE8azpF/zTZCAmcC6GCm" +
            "T5Z41OYXaIDOgDYqR668qdVZuoJDaTg==")] //tests "test.2" version
        public void DecryptToCorrectValue(string valueToTest) { //if this test fails, it means there's no backward compatibility

            //arrange
            string password = "rithwy*£^^ghg\"ytirhs%";
            using var crypto = new CryptoService(password.ToCharArray());

            string expectedResult = "test/ng this r@ndom valu?!";

            //act
            var encryptedValue = crypto.Decrypt(valueToTest);

            //assert
            Assert.Equal(expectedResult, encryptedValue);

        }

    }
}
