using PswManagerLibrary.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Global {
    public class PathsTests : IDisposable {

        static readonly string WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static readonly string ConfigFilePath = Path.Combine(WorkingDirectory, "Config.txt");

        public PathsTests() {
            //resets the file in case a past test run didn't dispose correctly
            if(File.Exists(ConfigFilePath)) {
                File.Delete(ConfigFilePath);
            }
        }

        [Fact]
        public void CorrectDefaultPaths() {

            //arrange
            bool configFileExisted;
            bool configFileExist;
            string expectedPasswordsFilePath = Path.Combine(WorkingDirectory, "Passwords.txt");
            string expectedAccountsFilePath = Path.Combine(WorkingDirectory, "Accounts.txt");
            string expectedEmailsFilePath = Path.Combine(WorkingDirectory, "Emails.txt");
            string expectedTokenFilePath = Path.Combine(WorkingDirectory, "Token.txt");

            //act
            configFileExisted = File.Exists(ConfigFilePath);
            IPaths paths = new Paths();
            configFileExist = File.Exists(ConfigFilePath);

            //assert
            Assert.False(configFileExisted);
            Assert.Equal(WorkingDirectory, Paths.WorkingDirectory);
            Assert.True(configFileExist);

            Assert.Equal(expectedPasswordsFilePath, paths.PasswordsFilePath);
            Assert.Equal(expectedAccountsFilePath, paths.AccountsFilePath);
            Assert.Equal(expectedEmailsFilePath, paths.EmailsFilePath);
            Assert.Equal(expectedTokenFilePath, paths.TokenFilePath);

        }

        public void Dispose() {
            if(File.Exists(ConfigFilePath)) {
                File.Delete(ConfigFilePath);
            }
        }
    }
}
