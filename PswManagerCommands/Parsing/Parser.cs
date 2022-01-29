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

        record ValidationResult(bool Success, string ErrorMessage);

        public IParserReady Setup<TParseable>() where TParseable : IParseable, new() {
            parseableType = typeof(TParseable);
            return this;
        }

        public ParsingResult Parse(string input) {

            ValidationResult inputValidationResult;
            if((inputValidationResult = ValidateInput(input)).Success == false)
                return new ParsingResult(ParsingResult.Success.Failure, inputValidationResult.ErrorMessage);

            //setup
            IParseable parseable = (IParseable)Activator.CreateInstance(parseableType);
            var dictionary = new Dictionary<string, Action<string>>();
            parseable.AddReferences(dictionary);

            var args = input.Split(Separator).Where(x => !string.IsNullOrWhiteSpace(x));
            var keys = args.Select(x => x.Split(Equal).First()).ToArray();
            var values = args.Select(x => x.Split(Equal).Skip(1).JoinStrings(Equal)).ToArray();
            
            Enumerable.Range(0, args.Count()).ForEach(x => dictionary[keys[x]].Invoke(values[x]));

            return new ParsingResult(ParsingResult.Success.Success, parseable);
        }

        private ValidationResult ValidateInput(string input) {
            var result = input.Contains(Separator);
            string errorMessage = result ? null : 
                $"Faulty format in the given command. Correct format:" +
                $"{Environment.NewLine}{Separator}argumentKey{Equal}value";
            return result? new ValidationResult(true, null) : new ValidationResult(false, errorMessage);
        }

        private ValidationResult ValidateArgs(IEnumerable<string> args) {
            var result = args.All(x => x.Contains(Equal));
            return result ? new ValidationResult(true, null) : new ValidationResult(false, 
                $"Faulty format in the given command. Correct format:" +
                $"{Environment.NewLine}{Separator}argumentKey{Equal}value");
        }

        private ValidationResult ValidateKeys(IEnumerable<string> keys, Dictionary<string, Action<string>> map) {
            if(keys.Any(x => !map.ContainsKey(x))) return new ValidationResult(false, "");
            if(keys.Distinct().Count() == keys.Count()) return new ValidationResult(false, "");

            return new ValidationResult(true, null);
        }

    }
}
