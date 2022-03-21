using PswManagerDatabase.Config;
using System.IO;

namespace PswManagerDatabase.DataAccess.TextDatabase.TextFileConnHelper {
    internal static class DatabaseHandler {

        public static void SetUpMissingFiles(IPaths paths) {
            CreateFileIfNotExistent(paths.AccountsFilePath);
            CreateFileIfNotExistent(paths.PasswordsFilePath);
            CreateFileIfNotExistent(paths.EmailsFilePath);
            CreateFileIfNotExistent(paths.TokenFilePath);
        }

        private static void CreateFileIfNotExistent(this string path) {
            if(File.Exists(path)) {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.Create(path).Dispose();
        }

    }
}
