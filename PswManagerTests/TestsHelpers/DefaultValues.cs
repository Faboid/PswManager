using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerTests.TestsHelpers {

    /// <summary>
    /// A class to standardize default values to insert in the tests database.
    /// </summary>
    public class DefaultValues {

        public DefaultValues(int generateAmount) {
            for(int i = 0; i < generateAmount; i++) {
                values.Add($"defaultName{i} defaultPassword{i} defaultEmail{i}");
            }

        }

        public readonly List<string> values = new List<string>();

        public string GetValue(int position, TypeValue type) {
            return values[position].Split(' ')[(int)type];
        }

        public enum TypeValue {
            Name = 0,
            Password = 1,
            Email = 2
        }

    }

}
