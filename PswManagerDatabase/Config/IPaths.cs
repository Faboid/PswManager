namespace PswManagerDatabase.Config {
    public interface IPaths {

        static string WorkingDirectory { get; }

        string PasswordsFilePath { get; }

        string AccountsFilePath { get; }

        string EmailsFilePath { get; }

        string TokenFilePath { get; }

        /// <summary>
        /// Changes the path to the accounts WITHOUT dealing with the current saved data. Any folder and files at the previous path will remain. The new directory must be existent.
        /// </summary>
        /// <param name="path">The path pointing to the new directory.</param>
        public void SetMain(string path);

        /// <summary>
        /// Moves the saved data to the new directory and sets it as the current database path. The new directory must be existent.
        /// </summary>
        /// <param name="path">The path pointing to the new directory.</param>
        public void MoveMain(string path);

    }
}
