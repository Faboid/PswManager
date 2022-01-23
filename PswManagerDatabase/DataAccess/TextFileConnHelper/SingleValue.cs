using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PswManagerDatabase.DataAccess.TextFileConnHelper {
    internal class SingleValue {

        internal static void Create(string path, string value) {
            File.AppendAllLines(path, new string[] { value });
        }

        internal static string Get(string path, int position) {
            return File.ReadAllLines(path).Skip(position).Take(1).First();
        }

        internal static void Edit(string path, int position, string value) {
            if(value is not null) {
                var list = File.ReadAllLines(path);
                list[position] = value;
                File.WriteAllLines(path, list);

            }
        }

        internal static void Delete(string path, int position) {
            List<string> list = File.ReadAllLines(path).ToList();
            list.RemoveAt(position);
            File.WriteAllLines(path, list);
        }

    }
}
