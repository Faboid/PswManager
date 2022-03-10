using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Unused.Parsing.Attributes {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ParseableKeyAttribute : Attribute {

        public string Key { get; init; }

        public ParseableKeyAttribute(string key) {
            Key = key;
        }

    }
}
