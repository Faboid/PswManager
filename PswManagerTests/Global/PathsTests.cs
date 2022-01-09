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
        static readonly string TempMainPath = Path.Combine(WorkingDirectory, "TempMainPath");
        static readonly string ConfigFilePath = Path.Combine(WorkingDirectory, "Config.txt");
        private record FourPaths {
            public string ExpectedPasswordsFilePath { get; set; }
            public string ExpectedAccountsFilePath { get; set; }
            public string ExpectedEmailsFilePath { get; set; }
            public string ExpectedTokenFilePath { get; set; }
        }

        public PathsTests() {
            //resets the files in case a past test run didn't dispose correctly
            Reset();
        }

        [Fact]
        public void CorrectDefaultPaths() {

            //arrange
            bool configFileExisted;
            bool configFileExist;
            FourPaths expectedPaths = GetExpectedFourPaths(WorkingDirectory);

            //act
            configFileExisted = File.Exists(ConfigFilePath);
            IPaths paths = new Paths();
            configFileExist = File.Exists(ConfigFilePath);

            //assert
            Assert.False(configFileExisted);
            Assert.Equal(WorkingDirectory, Paths.WorkingDirectory);
            Assert.True(configFileExist);

            AssertEqualPaths(expectedPaths, paths);

        }

        private static void AssertEqualPaths(FourPaths expectedPaths, IPaths paths) {
            Assert.Equal(expectedPaths.ExpectedPasswordsFilePath, paths.PasswordsFilePath);
            Assert.Equal(expectedPaths.ExpectedAccountsFilePath, paths.AccountsFilePath);
            Assert.Equal(expectedPaths.ExpectedEmailsFilePath, paths.EmailsFilePath);
            Assert.Equal(expectedPaths.ExpectedTokenFilePath, paths.TokenFilePath);
        }

        private static FourPaths GetExpectedFourPaths(string main) {
            FourPaths paths = new FourPaths();

            paths.ExpectedPasswordsFilePath = Path.Combine(main, "Passwords.txt");
            paths.ExpectedAccountsFilePath = Path.Combine(main, "Accounts.txt");
            paths.ExpectedEmailsFilePath = Path.Combine(main, "Emails.txt");
            paths.ExpectedTokenFilePath = Path.Combine(main, "Token.txt");

            return paths;
        }

        public void Dispose() {
            Reset();
        }

        private static void Reset() {
            DeleteFile(ConfigFilePath);
            DeleteFolder(TempMainPath);
        }

        private static void DeleteFolder(string path) {
            if(Directory.Exists(path)) {
                Directory.Delete(path);
            }
        }

        private static void DeleteFile(string path) {
            if(File.Exists(path)) {
                File.Delete(path);
            }
        }
    }
}
