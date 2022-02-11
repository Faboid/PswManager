using System;
using System.IO;
using Xunit;
using PswManagerLibrary.Storage;
using PswManagerLibrary.Cryptography;
using PswManagerDatabase;
using PswManagerLibrary.Commands.AutoCommands;

namespace PswManagerTests.TestsHelpers {

    public class TestsHelper : IDisposable {

        //todo - turn every static value into non-static.

        public static readonly TestsPaths Paths;
        public static readonly AutoInput AutoInput;
        public static readonly Token Token;
        public static readonly CryptoAccount CryptoAccount;
        public static readonly DefaultValues DefaultValues;
        public const string pswPassword = "pswpassword";
        public const string emaPassword = "emapassword";

        private static readonly AddCommand addCommand;

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

            CryptoAccount = new CryptoAccount(pswPassword, emaPassword);

            Token = new Token(CryptoAccount, Paths, AutoInput);

            addCommand = new AddCommand(new DataFactory(Paths).GetDataCreator(), CryptoAccount);

            //set up default values
            SetUpDefault();
        }

        public static void SetUpDefault() {
            //resets files to empty
            File.WriteAllText(Paths.AccountsFilePath, "");
            File.WriteAllText(Paths.PasswordsFilePath, "");
            File.WriteAllText(Paths.EmailsFilePath, "");

            //creates three default entries
            int entries = DefaultValues.values.Count;
            for(int i = 0; i < entries; i++) {
                addCommand.Run( new string[] {

                    DefaultValues.GetValue(i, DefaultValues.TypeValue.Name),
                    DefaultValues.GetValue(i, DefaultValues.TypeValue.Password),
                    DefaultValues.GetValue(i, DefaultValues.TypeValue.Email)
                });
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
