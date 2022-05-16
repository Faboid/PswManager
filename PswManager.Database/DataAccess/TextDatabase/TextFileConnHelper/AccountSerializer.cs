using PswManager.Database.Models;

namespace PswManager.Database.DataAccess.TextDatabase.TextFileConnHelper {
    internal class AccountSerializer {

        public static string[] Serialize(AccountModel account) {
            string[] output = new string[3];
            output[0] = account.Name;
            output[1] = account.Password;
            output[2] = account.Email;

            return output;
        }

        public static AccountModel Deserialize(string[] text) {
            return new(text[0], text[1], text[2]);
        }

    }
}
