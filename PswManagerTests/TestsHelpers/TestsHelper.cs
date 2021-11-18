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

namespace PswManagerTests.TestsHelpers {

    public class TestsHelper : IDisposable {

        //todo - insert static values that represent the default strings inserted in the fake database

        public static readonly TestsPaths paths;
        public static readonly CommandQuery query;
        public static readonly PasswordManager pswManager;
        public static readonly AutoInput autoInput;
        public static readonly Token token;
        public static readonly CryptoString passCryptoString;
        public static readonly CryptoString emaCryptoString;

        static TestsHelper() {
            //get non-existing path to create a folder
            paths = new TestsPaths();
            Directory.CreateDirectory(paths.WorkingDirectory);
            File.Create(paths.AccountsFilePath).Close();
            File.Create(paths.PasswordsFilePath).Close();
            File.Create(paths.EmailsFilePath).Close();

            //set the needed classes in public(or protected) class instances
            autoInput = new AutoInput();
            query = new CommandQuery(paths, autoInput);
            query.Start(new Command("psw pswpassword emapassword"));

            passCryptoString = new CryptoString("pswpassword");
            emaCryptoString = new CryptoString("emapassword");

            pswManager = new PasswordManager(paths, passCryptoString, emaCryptoString);
            token = new Token(passCryptoString, emaCryptoString, paths, autoInput);
        }

        public static void SetUpDefault() {
            //resets files to empty
            File.WriteAllText(paths.AccountsFilePath, "");
            File.WriteAllText(paths.PasswordsFilePath, "");
            File.WriteAllText(paths.EmailsFilePath, "");
            
            //creates three default entries
            query.Start(new Command("create defaultName1 defaultPassword1 defaultEmail1"));
            query.Start(new Command("create defaultName2 defaultPassword2 defaultEmail2"));
            query.Start(new Command("create defaultName3 defaultPassword3 defaultEmail3"));

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
