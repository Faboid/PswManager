using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase.Models {
    public class AccountModel {

        public AccountModel() {

        }

        public AccountModel(string name, string password, string email) {
            Name = name;
            Password = password;
            Email = email;
        }

        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

    }
}
