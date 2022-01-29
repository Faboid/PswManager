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

            if(!ValidationToParsingResult(ValidateInput(input), out ParsingResult inputValidationResult)) {
                return inputValidationResult;
            }

            //setup
            IParseable parseable = (IParseable)Activator.CreateInstance(parseableType);
            var dictionary = new Dictionary<string, Action<string>>();
            parseable.AddReferences(dictionary);

            //divide input into arguments
            var args = input.Split(Separator).Where(x => !string.IsNullOrWhiteSpace(x));

            if(!ValidationToParsingResult(ValidateArgs(args), out ParsingResult argsValidationResult)) {
                return argsValidationResult;
            }

            var keys = args.Select(x => x.Split(Equal).First()).ToArray();

            if(!ValidationToParsingResult(ValidateKeys(keys, dictionary), out ParsingResult keysValidationResult)) {
                return keysValidationResult;
            }
            
            var values = args.Select(x => x.Split(Equal).Skip(1).JoinStrings(Equal)).ToArray();
            
            Enumerable.Range(0, args.Count()).ForEach(x => dictionary[keys[x]].Invoke(values[x]));

            return new ParsingResult(ParsingResult.Success.Success, parseable);
        }

        private bool ValidationToParsingResult(ValidationResult result, out ParsingResult parsingResult) {

            parsingResult = result.Success switch {
                false => new ParsingResult(ParsingResult.Success.Failure, result.ErrorMessage),
                true => new ParsingResult(ParsingResult.Success.Success, result.ErrorMessage)
            };

            return result.Success;
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
            if(keys.Any(x => !map.ContainsKey(x)))
                return new ValidationResult(false,
                    $"Inexistent parameter has been given. List of possible parameters for this command:" +
                    $"{Environment.NewLine}{map.Keys.JoinStrings(' ')}");
            if(keys.Distinct().Count() != keys.Count())
                return new ValidationResult(false, "Duplicate parameters aren't allowed.");

            return new ValidationResult(true, null);
        }

    }
}
