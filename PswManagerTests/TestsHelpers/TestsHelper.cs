using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Storage;
using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Factories;

namespace PswManagerTests.TestsHelpers {

    public class TestsHelper : IDisposable {

        //todo - turn every static value into non-static.

        public static readonly TestsPaths paths;
        public static readonly CommandQuery query;
        public static readonly PasswordManager pswManager;
        public static readonly AutoInput autoInput;
        public static readonly Token token;
        public static readonly CryptoAccount cryptoAccount;
        public static readonly DefaultValues defaultValues;

        static TestsHelper() {
            //get non-existing path to create a folder
            paths = new TestsPaths();
            Directory.CreateDirectory(paths.WorkingDirectory);
            File.Create(paths.AccountsFilePath).Close();
            File.Create(paths.PasswordsFilePath).Close();
            File.Create(paths.EmailsFilePath).Close();

            //set the needed classes in public(or protected) class instances
            defaultValues = new DefaultValues(5);
            
            autoInput = new AutoInput();
            query = new CommandQuery(paths, autoInput, new PasswordManagerFactory());
            query.Start(new Command("psw pswpassword emapassword"));

            cryptoAccount = new CryptoAccount("pswpassword", "emapassword");

            pswManager = new PasswordManager(paths, cryptoAccount);
            token = new Token(cryptoAccount, paths, autoInput);

            //set up default values
            SetUpDefault();
        }

        public static void SetUpDefault() {
            //resets files to empty
            File.WriteAllText(paths.AccountsFilePath, "");
            File.WriteAllText(paths.PasswordsFilePath, "");
            File.WriteAllText(paths.EmailsFilePath, "");
            
            //creates three default entries
            foreach(string value in defaultValues.values) {
                query.Start($"create {value}");
            }

            autoInput.SetDefault();
        }

        public void Dispose() {

            //delete the created folder and all its contents
            Directory.Delete(paths.WorkingDirectory, true);

        }

    }

    [CollectionDefinition("TestHelperCollection")]
    public class TestHelperCollection : ICollectionFixture<TestsHelper> {
        
    }

}
