using PswManager.Encryption.Cryptography;
using PswManager.Encryption.Services;
using PswManager.Core.Cryptography;
using Xunit;
using PswManager.TestUtils;

namespace PswManager.Core.Tests.Cryptography {
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

        [Fact]
        public void ThrowArgExceptionIfKeysAreEqual() {

            //arrange
            var passKey = new Key("TestHere".ToCharArray());
            var emaKey = new Key("TestHere".ToCharArray());

            //act & assert
            Assert.Throws<ArgumentException>(() => new CryptoAccount(passKey, emaKey));

        }

        [Fact]
        public async Task InitializeAsync() {

            //arrange
            OrderChecker orderChecker = new();

            static async Task<Key> GetKeyAsync(int value, OrderChecker orderChecker) {
                await orderChecker.WaitForAsync(value, 50);
                return new Key(new string((char)value, 30).ToCharArray());
            }

            //act
            var account = new CryptoAccount(GetKeyAsync(1, orderChecker), GetKeyAsync(2, orderChecker));
            var passTask = account.GetPassCryptoServiceAsync();
            var emaTask = account.GetEmaCryptoServiceAsync();
            bool isPassWaiting = !passTask.IsCompleted;
            bool isEmaWaiting = !emaTask.IsCompleted;
            _ = Task.Factory.StartNew(async () => {
                await Task.Delay(10);
                orderChecker.Done(1);
                await Task.Delay(10);
                orderChecker.Done(2);
            });
            var passCrypto = await passTask;
            var emaCrypto = emaTask.Result;

            //assert
            Assert.NotNull(account);
            Assert.True(isPassWaiting);
            Assert.True(isEmaWaiting);
            Assert.True(passCrypto.GetType() == typeof(CryptoService));
            Assert.True(emaCrypto.GetType() == typeof(CryptoService));

        }

    }
}
