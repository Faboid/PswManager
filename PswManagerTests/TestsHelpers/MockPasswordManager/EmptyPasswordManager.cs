using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Global;
using PswManagerLibrary.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerTests.TestsHelpers.MockPasswordManager {
    public class EmptyPasswordManager : IPasswordManager {

        public void CreatePassword(string name, string password, string email) {
            
        }

        public void DeletePassword(string name) {
            
        }

        public void EditPassword(string name, string[] arguments) {
            
        }

        public string GetPassword(string name) {
            return "";
        }
    }
}
