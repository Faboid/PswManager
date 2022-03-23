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
            using var crypto = new CryptoService(password.ToCharArray(), "test.1");
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
        [InlineData("KAB0AGUAcwB0AC4AMQApAMHiVZLLhhsr21IRWXhBMA1dl" +
            "7NGjX8eyeco1IPAaNdecReMVFyX5cyzRlgzbu5JRhHD+cveMVfGdC" +
            "//pcLQbLdohmAcabrfIa4zbFSDP1yo3j4AijyErbBPska2zCF6lxG" +
            "4//78jI85og==")] //tests "test.1" version
        [InlineData("KAB0AGUAcwB0AC4AMgApAFvPFlTN5Fy9xhrE0iASWLTX5" +
            "4PacfJGucsz4H+A77wHvdYkCciaReI+73c/d5LUGE6o5GMZIW0Q3A" +
            "/8MS+Yw8k+fymbA1vkYCtm/Z5fGN/rpvpOVG4z0S7AByQNwvBIr9R" +
            "ZzeVb1az6wA==")] //tests "test.2" version
        public void DecryptToCorrectValue(string valueToTest) { //if this test breaks, it means there's no backward compatibility

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
