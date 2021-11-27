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

        public static readonly TestsPaths Paths;
        public static readonly CommandQuery Query;
        public static readonly PasswordManager PswManager;
        public static readonly AutoInput AutoInput;
        public static readonly Token Token;
        public static readonly CryptoAccount CryptoAccount;
        public static readonly DefaultValues DefaultValues;

        static TestsHelper() {
            //get non-existing path to create a folder
            Paths = new TestsPaths();
            Directory.CreateDirectory(Paths.WorkingDirectory);
            File.Create(Paths.AccountsFilePath).Close();
            File.Create(Paths.PasswordsFilePath).Close();
            File.Create(Paths.EmailsFilePath).Close();

            //set the needed classes in public(or protected) class instances
            DefaultValues = new DefaultValues(5);
            
            AutoInput = new AutoInput();
            Query = new CommandQuery(Paths, AutoInput, new PasswordManagerFactory(new CryptoAccountFactory()));
            Query.Start(new Command("psw pswpassword emapassword"));

            CryptoAccount = new CryptoAccount("pswpassword", "emapassword");

            PswManager = new PasswordManager(Paths, CryptoAccount);
            Token = new Token(CryptoAccount, Paths, AutoInput);

            //set up default values
            SetUpDefault();
        }

        public static void SetUpDefault() {
            //resets files to empty
            File.WriteAllText(Paths.AccountsFilePath, "");
            File.WriteAllText(Paths.PasswordsFilePath, "");
            File.WriteAllText(Paths.EmailsFilePath, "");
            
            //creates three default entries
            foreach(string value in DefaultValues.values) {
                Query.Start($"create {value}");
            }

            AutoInput.SetDefault();
        }

        public void Dispose() {

            //delete the created folder and all its contents
            Directory.Delete(Paths.WorkingDirectory, true);

        }

    }

    [CollectionDefinition("TestHelperCollection")]
    public class TestHelperCollection : ICollectionFixture<TestsHelper> {
        
    }

}
