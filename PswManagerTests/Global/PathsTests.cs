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
        static readonly string TempMainPath = Path.Combine(WorkingDirectory, "ewfsgeweqrqrqrttwqagrthhrjyjewrew"); //gibberish to make sure it's not an existing folder
        static readonly string ConfigFilePath = Path.Combine(WorkingDirectory, "Config.txt");

        private record FourPaths (string MainPath) {

            public readonly string ExpectedPasswordsFilePath = Path.Combine(MainPath, "Passwords.txt");
            public readonly string ExpectedAccountsFilePath = Path.Combine(MainPath, "Accounts.txt");
            public readonly string ExpectedEmailsFilePath = Path.Combine(MainPath, "Emails.txt");
            public readonly string ExpectedTokenFilePath = Path.Combine(MainPath, "Token.txt");

            public void ForEach(Action<string> action) {
                action.Invoke(ExpectedPasswordsFilePath);
                action.Invoke(ExpectedAccountsFilePath);
                action.Invoke(ExpectedEmailsFilePath);
                action.Invoke(ExpectedTokenFilePath);
            }

            public void Create() => ForEach(x => File.Create(x).Dispose());
            public void Delete() => ForEach(x => File.Delete(x));
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
            FourPaths expectedPaths = new(WorkingDirectory);

            //act
            configFileExisted = File.Exists(ConfigFilePath);
            Paths paths = new Paths();
            configFileExist = File.Exists(ConfigFilePath);

            //assert
            Assert.False(configFileExisted);
            Assert.Equal(WorkingDirectory, Paths.WorkingDirectory);
            Assert.True(configFileExist);

            AssertEqualPaths(expectedPaths, paths);

        }

        [Fact]
        public void SetMain_ShouldChangeMain() {

            //arrange
            Paths paths = new Paths();
            FourPaths expectedPaths = new(TempMainPath);

            //act
            Directory.CreateDirectory(TempMainPath);
            paths.SetMain(TempMainPath);

            //assert
            AssertEqualPaths(expectedPaths, paths);
            Directory.Delete(TempMainPath, true);

        }

        [Fact]
        public void MoveMain_ShouldMove() {

            //arrange
            Paths paths = new Paths();
            FourPaths previousPaths = new(WorkingDirectory);
            FourPaths expectedPaths = new(TempMainPath);

            //act
            Directory.CreateDirectory(TempMainPath);
            previousPaths.Create();
            paths.MoveMain(TempMainPath);

            //assert
            AssertEqualPaths(expectedPaths, paths);
            previousPaths.ForEach(x => Assert.False(File.Exists(x)));
            expectedPaths.ForEach(x => Assert.True(File.Exists(x)));
            expectedPaths.Delete();
            Directory.Delete(TempMainPath, true);

        }

        private static void AssertEqualPaths(FourPaths expectedPaths, IPaths paths) {
            Assert.Equal(expectedPaths.ExpectedPasswordsFilePath, paths.PasswordsFilePath);
            Assert.Equal(expectedPaths.ExpectedAccountsFilePath, paths.AccountsFilePath);
            Assert.Equal(expectedPaths.ExpectedEmailsFilePath, paths.EmailsFilePath);
            Assert.Equal(expectedPaths.ExpectedTokenFilePath, paths.TokenFilePath);
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
                Directory.Delete(path, true);
            }
        }

        private static void DeleteFile(string path) {
            if(File.Exists(path)) {
                File.Delete(path);
            }
        }
    }
}
