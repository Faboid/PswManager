using PswManagerCommands.Parsing.Helpers;
using PswManagerHelperMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Parsing {
    public class Parser : IParser, IParserReady {

        public static IParser CreateInstance() { return new Parser(); }

        private Parser() {}

        public string Separator => " -/";
        public char Equal => '=';

        private Type parseableType;

        internal record ValidationResult(bool Success, string ErrorMessage);

        public IParserReady Setup<TParseable>() where TParseable : IParseable, new() {
            parseableType = typeof(TParseable);
            return this;
        }

        public ParsingResult Parse(string input) {

            //todo - fix this terrifying way of validating input
            if(!input.ValidateInput(Separator, Equal).ToParsingResult(out ParsingResult inputValidationResult)) {
                return inputValidationResult;
            }

            //setup
            IParseable parseable = (IParseable)Activator.CreateInstance(parseableType);
            var dictionary = new Dictionary<string, Action<string>>();
            parseable.AddReferences(dictionary);

            var args = input.GetArgs(Separator);

            if(!args.ValidateArgs(Separator, Equal).ToParsingResult(out ParsingResult argsValidationResult)) {
                return argsValidationResult;
            }

            var keys = args.GetKeys(Equal).ToArray();

            if(!keys.ValidateKeys(dictionary).ToParsingResult(out ParsingResult keysValidationResult)) {
                return keysValidationResult;
            }
            
            var values = args.GetValues(Equal).ToArray();
            
            Enumerable.Range(0, args.Count()).ForEach(x => dictionary[keys[x]].Invoke(values[x]));

            return new ParsingResult(ParsingResult.Success.Success, parseable);
        }

    }
}
