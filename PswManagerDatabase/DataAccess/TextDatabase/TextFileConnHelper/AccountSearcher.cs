using System.IO;

namespace PswManagerDatabase.DataAccess.TextDatabase.TextFileConnHelper {
    internal class AccountSearcher {

        readonly IPaths paths;

        internal AccountSearcher(IPaths paths) {
            this.paths = paths;
        }

        /// <summary>
        /// Returns the position of the name. If it doesn't find any, returns null.
        /// </summary>
        internal int? SearchByName(string name) {
            int position = 0;

            using(var reader = new StreamReader(paths.AccountsFilePath)) {
                string current;
                while((current = reader.ReadLine()) != null) {
                    if(current == name) {
                        return position;
                    }
                    position++;
                }
            }

            //didn't find any match
            return null;
        }

        internal bool AccountExist(string name) => File.Exists(paths.AccountsFilePath) && SearchByName(name) != null;

        internal bool AccountExist(string name, out int position) {
            position = -1;

            if(!File.Exists(paths.AccountsFilePath)) {
                return false;
            }

            int? temp = SearchByName(name);
            if(temp == null)
                return false;

            position = (int)temp;

            return true;
        }

    }
}
