using PswManagerHelperMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Parsing {
    public class Parser : IParser {

        readonly Dictionary<string, VariableReference<string>> dictionary = new Dictionary<string, VariableReference<string>>();

        readonly IParseable parsedObject;

        public Parser(IParseable emptyObject) {
            parsedObject = emptyObject;
        }


        public IParseable TryParse(string input) {
            var args = input.Split(" -/");
            var keys = args.Select(x => x.Split(':').First()).ToArray();
            var values = args.Select(x => x.Split(':').Skip(1).JoinStrings(" -/")).ToArray();
            
            Enumerable.Range(0, args.Length).ForEach(x => dictionary[keys[x]].Set(values[x]));

            return parsedObject;
        }

        public void Register(string key, VariableReference<string> reference) {
            dictionary.Add(key, reference);
        }

    }
}
