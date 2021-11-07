using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests {

    public class TestsHelper : IDisposable {

        private TestsPaths paths;

        public TestsHelper() {
            //get non-existing path to create a folder
            paths = new TestsPaths();
            Directory.CreateDirectory(paths.WorkingDirectory);
            File.Create(paths.AccountsFilePath);
            File.Create(paths.PasswordsFilePath);
            File.Create(paths.EmailsFilePath);

            //todo - set the needed classes in public(or protected) class instances

            throw new NotImplementedException();
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
