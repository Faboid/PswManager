using PswManagerLibrary.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary {

    /// <summary>
    /// A model that stores a single account's information.
    /// </summary>
    public sealed class AccountModel {

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
