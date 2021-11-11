using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Global {
    public interface IPaths {

        static string WorkingDirectory { get; }

        string PasswordsFilePath { get; }

        string AccountsFilePath { get; }

        string EmailsFilePath { get; }

        string TokenFilePath { get; }
    }
}
