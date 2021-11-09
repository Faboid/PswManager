using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Storage;

namespace PswManagerTests {

    public class TestsHelper : IDisposable {

        public static readonly TestsPaths paths;
        public static readonly CommandQuery query;
        public static readonly PasswordManager pswManager;

        static TestsHelper() {
            //get non-existing path to create a folder
            paths = new TestsPaths();
            Directory.CreateDirectory(paths.WorkingDirectory);
            File.Create(paths.AccountsFilePath).Close();
            File.Create(paths.PasswordsFilePath).Close();
            File.Create(paths.EmailsFilePath).Close();

            //set the needed classes in public(or protected) class instances
            query = new CommandQuery(paths);
            query.Start(new Command("psw pswpassword emapassword"));

            pswManager = new PasswordManager(paths, "pswpassword", "emapassword");
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
