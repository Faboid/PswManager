using PswManager.Async;
using PswManager.Database.Models;
using PswManager.Encryption.Cryptography;
using PswManager.Encryption.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PswManager.Core.Cryptography {
    public class CryptoAccount : ICryptoAccount {

        public CryptoAccount(char[] passPassword, char[] emaPassword) {
            PassCryptoString = new CryptoService(passPassword);
            EmaCryptoString = new CryptoService(emaPassword);
        }

        public CryptoAccount(Key passKey, Key emaKey) {
            if(Enumerable.SequenceEqual(passKey.Get(), emaKey.Get())) {
                throw new ArgumentException("The given keys must be different.");
            }

            PassCryptoString = new CryptoService(passKey);
            EmaCryptoString = new CryptoService(emaKey);
        }

        public CryptoAccount(ICryptoService passCryptoString, ICryptoService emaCryptoString) {
            PassCryptoString = new(passCryptoString);
            EmaCryptoString = new(emaCryptoString);
        }

        public CryptoAccount(Task<Key> passKeyTask, Task<Key> emaKeyTask) : this (
            passKeyTask.ContinueWith(async x => new CryptoService(await x) as ICryptoService).Unwrap(),
            emaKeyTask.ContinueWith(async x => new CryptoService(await x) as ICryptoService).Unwrap()
        ) { }

        public CryptoAccount(Task<ICryptoService> passKeyTask, Task<ICryptoService> emaKeyTask) {
            PassCryptoString = new(() => passKeyTask);
            EmaCryptoString = new(() => emaKeyTask);
        }

        private AsyncLazy<ICryptoService> PassCryptoString { get; }
        private AsyncLazy<ICryptoService> EmaCryptoString { get; }

        public ICryptoService GetPassCryptoService() => PassCryptoString.Value.Result;
        public ICryptoService GetEmaCryptoService() => EmaCryptoString.Value.Result;
        public Task<ICryptoService> GetPassCryptoServiceAsync() => EmaCryptoString.Value;
        public Task<ICryptoService> GetEmaCryptoServiceAsync() => PassCryptoString.Value;


        public (string encryptedPassword, string encryptedEmail) Encrypt(string password, string email) {
            return (GetPassCryptoService().Encrypt(password), GetEmaCryptoService().Encrypt(email));
        }

        public (string decryptedPassword, string decryptedEmail) Decrypt(string encryptedPassword, string encryptedEmail) {
            return (GetPassCryptoService().Decrypt(encryptedPassword), GetEmaCryptoService().Decrypt(encryptedEmail));
        }

        public (string encryptedPassword, string encryptedEmail) Encrypt((string password, string email) values) => Encrypt(values.password, values.email);
        public (string decryptedPassword, string decryptedEmail) Decrypt((string encryptedPassword, string encryptedEmail) values) => Decrypt(values.encryptedPassword, values.encryptedEmail);

        //todo - unit test these two methods
        public AccountModel Encrypt(AccountModel model) {
            AccountModel output = new(model.Name, model.Password, model.Email);

            if(!string.IsNullOrWhiteSpace(output.Password)) {
                output.Password = GetPassCryptoService().Encrypt(output.Password);
            }
            if(!string.IsNullOrWhiteSpace(output.Email)) {
                output.Email = GetEmaCryptoService().Encrypt(output.Email);
            }

            return output;
        }

        public AccountModel Decrypt(AccountModel model) {
            AccountModel output = new(model.Name, model.Password, model.Email);

            if(!string.IsNullOrWhiteSpace(output.Password)) {
                output.Password = GetPassCryptoService().Decrypt(output.Password);
            }

            if(!string.IsNullOrWhiteSpace(output.Email)) {
                output.Email = GetEmaCryptoService().Decrypt(output.Email);
            }

            return output;
        }
    }
}
