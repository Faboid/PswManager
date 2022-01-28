using PswManagerHelperMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Parsing {
    public class Parser : IParser {

        readonly Dictionary<string, VariableReference<string>> dictionary = new Dictionary<string, VariableReference<string>>();
        
        public string Separator => " -/";

        public bool TryParse<TParseable>(string input, out TParseable parseable) where TParseable : class, IParseable, new() {
            //setup
            parseable = Activator.CreateInstance(typeof(TParseable)) as TParseable;
            dictionary.Clear();
            parseable.RegisterAll(this);

            var args = input.Split(Separator).Where(x => !string.IsNullOrWhiteSpace(x));
            var keys = args.Select(x => x.Split(':').First()).ToArray();
            var values = args.Select(x => x.Split(':').Skip(1).JoinStrings(Separator)).ToArray();
            
            Enumerable.Range(0, args.Count()).ForEach(x => dictionary[keys[x]].Set(values[x]));

            return true;
        }

        public void Register(string key, VariableReference<string> reference) {
            dictionary.Add(key, reference);
        }

    }
}
