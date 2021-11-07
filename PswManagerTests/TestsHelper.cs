using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PswManagerLibrary.Commands;

namespace PswManagerTests {

    public class TestsHelper : IDisposable {

        private TestsPaths paths;
        private CommandQuery query;

        public TestsHelper() {
            //get non-existing path to create a folder
            paths = new TestsPaths();
            Directory.CreateDirectory(paths.WorkingDirectory);
            File.Create(paths.AccountsFilePath);
            File.Create(paths.PasswordsFilePath);
            File.Create(paths.EmailsFilePath);

            //set the needed classes in public(or protected) class instances
            query = new CommandQuery(paths);
            query.Start(new Command("psw pswpassword email password"));

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
