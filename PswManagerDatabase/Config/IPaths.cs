namespace PswManagerDatabase.Config {
    public interface IPaths {

        static string WorkingDirectory { get; }

        string PasswordsFilePath { get; }

        string AccountsFilePath { get; }

        string EmailsFilePath { get; }

        string TokenFilePath { get; }

        public void SetMain(string path);
        public void MoveMain(string path);

    }
}
