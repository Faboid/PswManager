using PswManager.Database.Models;
using PswManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManager.Tests.TestsHelpers {

    /// <summary>
    /// A class to standardize default values to insert in the tests database.
    /// </summary>
    [Obsolete("Use the DefaultValues in PswManager.TestUtils instead.")]
    public class DefaultValues {

        public static string StaticGetValue(int position, TypeValue type) {
            var values = new DefaultValues(position + 1);
            return values.GetValue(position, type);
        }

        public static string[] SplitValues(string values) {
            return values.Split(' ');
        }

        public static AccountModel ToAccount(string values) {
            var splitValues = SplitValues(values);
            var account = new AccountModel() {
                Name = splitValues[0],
                Password = splitValues[1],
                Email = splitValues[2]
            };

            return account;
        }

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

        public IEnumerable<AccountModel> GetSome(int num) {
            return GetAll().Take(num);
        }

        public IEnumerable<AccountModel> GetAll() {
            return values
                .Select(x => x.Split(' '))
                .Select(x => new AccountModel(x[0], x[1], x[2]));
        }

        public enum TypeValue {
            Name = 0,
            Password = 1,
            Email = 2
        }

    }

}
