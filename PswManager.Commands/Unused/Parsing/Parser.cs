using PswManager.Commands.Unused.Parsing.Helpers;
using PswManagerHelperMethods;
using System;
using System.Linq;

namespace PswManager.Commands.Unused.Parsing {
    public class Parser : IParser, IParserReady {

        public static IParser CreateInstance() { return new Parser(); }

        private Parser() { }

        public string Separator => " -/";
        public char Equal => '=';

        private Type parseableType;

        private ValueSetter valueSetter;

        internal record ValidationResult(bool Success, string ErrorMessage);

        public IParserReady Setup<TParseable>() where TParseable : new() {
            parseableType = typeof(TParseable);
            valueSetter = ValueSetter.CreateInstance<TParseable>();
            return this;
        }

        public ParsingResult Parse(string input) {

            //fix this terrifying way of validating input
            if(!input.ValidateInput(Separator, Equal).ToParsingResult(out ParsingResult inputValidationResult)) {
                return inputValidationResult;
            }

            //setup
            ICommandInput parseable = (ICommandInput)Activator.CreateInstance(parseableType);

            var args = input.GetArgs(Separator);

            if(!args.ValidateArgs(Separator, Equal).ToParsingResult(out ParsingResult argsValidationResult)) {
                return argsValidationResult;
            }

            var keys = args.GetKeys(Equal).ToArray();

            var values = args.GetValues(Equal).ToArray();

            var valid = Enumerable.Range(0, args.Count()).Select(x => valueSetter.TryAssignValue(parseable, keys[x], values[x]));
            if(!valid.All(x => x == true)) {
                return new ParsingResult(ParsingResult.Success.Failure,
                    $"Inexistent parameter has been given. List of possible parameters for this command:" +
                    $"{Environment.NewLine}{valueSetter.dictionary.Keys.JoinStrings(' ')}");
            }

            if(keys.Distinct().Count() < keys.Length) {
                return new ParsingResult(ParsingResult.Success.Failure, "Duplicate parameters aren't allowed.");
            }

            return new ParsingResult(ParsingResult.Success.Success, parseable);
        }

    }
}
