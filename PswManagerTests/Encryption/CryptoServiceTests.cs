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
        public void EncryptToCipherText() {

            //arrange
            string password = "rithwy*£^^ghg\"ytirhs%";
            using var crypto = new CryptoService(password.ToCharArray());
            string valueToTest = "test/ng this r@ndom valu?!";

            //act
            var encryptedValue = crypto.Encrypt(valueToTest);

            //assert
            Assert.NotEqual(valueToTest, encryptedValue);

        }

        [Theory]
        [InlineData("KAB0AGUAcwB0AC4AMQApACTeLzp0S8GwoO7A37od0KKcU" +
            "iMW80XmZlNHr7+t9eARkjqrC2KW7lDOdwqy2lQh1nolUgKwbw+nQ6" +
            "1oR0iNGydP8s1kAHYwFY+96SneAnc+iOpvJKUBiufFUiRS5ptUaCP" +
            "EnKyl0fsqPexWFOvDt/1Bv9skkb/FxD4qwpanih5v/mp9g3ZpSPLJ" +
            "ZNx2qZCJFe4AlOJnjGeL328K2Dks6jiSWQR+3JddNqFnGzte+P2Bg" +
            "v2iUngFzeCU7z8RiMpTFkN9cNxY7ZGMVWWUGfqiZyNkO8vfR8vQUH" +
            "YRfAZyEIRAgtm8I4HUdPUABP2weY3k6g9FYf0ZMIsiGs3imbspdrq" +
            "bTK66AJRy8a24dcw=")] //tests "test.1" version
        [InlineData("KAB0AGUAcwB0AC4AMgApAEVyNd93PloFD4cdm3CnwdG7u" +
            "UASWgRU177lIox04GOvXVtMlYHi2eXm3Z7BFs6pvPZRpmUE779mas" +
            "LQNCZctaznPrNGDHAwe0esaS0RRccAu90te0QOrn8MDvTfnusYPBw" +
            "RLYEqbLsLSJPJmyRExXdEs2bz9v8OW+sgn9ohM9rYT0KSJtZZDsLT" +
            "mWJo824r03ihdZQKE2+sC6gcWnwGMaZkvpEN6YctgCzysbU1VoUBd" +
            "hfCrrRA1IuJ0hA8iHKb0OragE7MkujspTk0xsrlYSEEWusT8YtCP0" +
            "OBLuCUdigB8WFd7N/I+xs/KuYDc771+FMW5FsK4/epOsMl9tQo5Si" +
            "cr8BV2u2MwyHWgd4=")] //tests "test.2" version
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
