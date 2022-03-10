using PswManagerCommands.Unused.Parsing;
using PswManagerHelperMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PswManagerCommands.Unused.Parsing.Parser;

namespace PswManagerCommands.Unused.Parsing.Helpers {
    internal static class InputValidator {

        public static bool ToParsingResult(this ValidationResult result, out ParsingResult parsingResult) {

            parsingResult = result.Success switch {
                false => new ParsingResult(ParsingResult.Success.Failure, result.ErrorMessage),
                true => new ParsingResult(ParsingResult.Success.Success, result.ErrorMessage)
            };

            return result.Success;
        }

        public static ValidationResult ValidateInput(this string input, string separator, char equalSign) {
            var result = input.Contains(separator);
            string errorMessage = result ? null :
                $"Faulty format in the given command. Correct format:" +
                $"{Environment.NewLine}{separator}argumentKey{equalSign}value";
            return result ? new ValidationResult(true, null) : new ValidationResult(false, errorMessage);
        }

        public static ValidationResult ValidateArgs(this IEnumerable<string> args, string separator, char equalSign) {
            var result = args.All(x => x.Contains(equalSign));
            return result ? new ValidationResult(true, null) : new ValidationResult(false,
                $"Faulty format in the given command. Correct format:" +
                $"{Environment.NewLine}{separator}argumentKey{equalSign}value");
        }

        public static ValidationResult ValidateKeys(this IEnumerable<string> keys, Dictionary<string, Action<string>> map) {
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
