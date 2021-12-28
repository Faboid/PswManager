using System.Collections.Generic;

namespace PswManagerTests.TestsHelpers {

    /// <summary>
    /// A class to standardize default values to insert in the tests database.
    /// </summary>
    public class DefaultValues {

        public DefaultValues(int generateAmount) {
            var values = new List<string>();

            for(int i = 0; i < generateAmount; i++) {
                values.Add($"defaultName{i} defaultPassword{i} defaultEmail{i}");
            }

            this.values = values.AsReadOnly();
        }

        public readonly IList<string> values;

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
