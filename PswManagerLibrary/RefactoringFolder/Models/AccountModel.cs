using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.RefactoringFolder.Models {

    /// <summary>
    /// A model that stores a single account's information.
    /// </summary>
    public sealed class AccountModel {

        public AccountModel(IPaths paths, int position) {

        }

        public AccountModel(string name, string email = null, string password = null) {
            Name = name;
            Email = email;
            Password = password;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
