using PswManagerCommands.Parsing.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Parsing.Helpers {
    internal class ValueSetter {

        public static ValueSetter CreateInstance<TParseable>() where TParseable : ICommandArguments, new() => new(typeof(TParseable));

        public readonly IReadOnlyDictionary<string, PropertyInfo> dictionary;

        private ValueSetter(Type type) {
            var props = type.GetProperties();
            dictionary = props
                .Where(x => HasKey(x))
                .ToDictionary(x => GetKey(x), x => x);
        }

        public bool TryAssignValue(ICommandArguments parseable, string key, string value) {
            if(!dictionary.TryGetValue(key, out var propertyInfo)) {
                return false;
            }

            propertyInfo.SetValue(parseable, value);
            return true;
        }

        private static bool HasKey(PropertyInfo propertyInfo) {
            return propertyInfo.GetCustomAttributes(typeof(ParseableKeyAttribute)).Any();
        }

        private static string GetKey(PropertyInfo propertyInfo) {
            var attributes = propertyInfo.GetCustomAttributes(typeof(ParseableKeyAttribute));

            return (attributes.First() as ParseableKeyAttribute).Key;
        }

    }
}
