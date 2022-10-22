using System.Threading.Tasks;
using System;
using System.IO.Abstractions;

namespace PswManager.Core.Password;

internal class PasswordStatusChecker {

    internal enum PasswordStatus {

        /// <summary>
        /// This stage should never happen. If the value is <see cref="Unknown"/>, there is a bug in the process.
        /// </summary>
        Unknown,

        /// <summary>
        /// The password-changing operation has not started yet. 
        /// Currently backuping up the Data directory into the Buffer directory and encrypting the accounts with the new key. 
        /// Safe to terminate.
        /// </summary>
        Starting,

        /// <summary>
        /// The <see cref="Starting"/> stage is complete, and the new values are overwriting the existing ones.
        /// The token is being set up with the new key.
        /// Terminating the operation now could lead to a loss of data.
        /// </summary>
        Pending,

        /// <summary>
        /// The <see cref="Pending"/> stage has failed, and everything is getting rolled back.
        /// Terminating the operation now could lead to a loss of data.
        /// </summary>
        Failed,

        /// <summary>
        /// There is no status currently set.
        /// </summary>
        None,
    }

    private readonly IFileInfo _passwordStatusFile;

    public PasswordStatusChecker(IFileInfo passwordStatusFile) {
        _passwordStatusFile = passwordStatusFile;
    }

    /// <summary>
    /// Sets the status saved in <see cref="_passwordStatusFile"/> to <paramref name="status"/>.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    internal Task SetStatusTo(PasswordStatus status) {
        return _passwordStatusFile.FileSystem.File.WriteAllTextAsync(_passwordStatusFile.FullName, status.ToString());
    }

    /// <summary>
    /// Gets the current status.
    /// </summary>
    /// <returns></returns>
    internal async Task<PasswordStatus> GetStatus() {

        if(!_passwordStatusFile.Exists) {
            return PasswordStatus.None;
        }

        var status = await _passwordStatusFile.FileSystem.File.ReadAllTextAsync(_passwordStatusFile.FullName);
        return Enum.Parse<PasswordStatus>(status);
    }

    /// <summary>
    /// Deletes the status file.
    /// </summary>
    internal void Free() {
        _passwordStatusFile.Delete();
    }

}