using PswManagerHelperMethods;
using PswManagerLibrary.InputBuilder.Attributes;
using PswManagerLibrary.UIConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.InputBuilder {
    public class Requester {

        private readonly IReadOnlyCollection<(PropertyInfo prop, RequestAttribute attr)> required;
        private readonly IReadOnlyCollection<(PropertyInfo prop, RequestAttribute attr)> optional;
        readonly private IUserInput userInput;
        readonly private Type type;

        public Requester(Type type, IUserInput userInput) {
            this.userInput = userInput;
            this.type = type;
            
            var props = type.GetProperties()
                .Select(prop => (prop, prop.GetCustomAttribute<RequestAttribute>()))
                .Where(x => x.Item2 != null);

            required = props
                .Where(x => x.Item2.Optional == false)
                .ToList();

            optional = props
                .Where(x => x.Item2.Optional == true)
                .ToList();
        }

        public (bool success, object obj) Build() {
            var output = Activator.CreateInstance(type);

            //I dislike throwing an exception to exit the operation.
            //I might change this to a foreach with a custom exit bool later to handle it without exceptions
            try {

                required
                    .Select(x => (x.prop, AskRequired(x.attr)))
                    .ForEach(x => x.prop.SetValue(output, x.Item2));

                optional
                    .Select(x => (x.prop, Request(x.attr, out string answer), answer))
                    .Where(x => x.Item2)
                    .ForEach(x => x.prop.SetValue(output, x.answer));

            } catch (InputExitedException) {
                return (false, default);
            }

            if(required.Any(x => string.IsNullOrWhiteSpace((string)x.prop.GetValue(output)))) {
                userInput.SendMessage("One or more required values got skipped.");
                return (false, default);
            }

            required.Concat(optional)
                .ForEach(x => userInput.SendMessage($"{x.attr.DisplayName}: {x.prop.GetValue(output) ?? "N/A" }"));

            return (userInput.YesOrNo("Are these values correct?"), output);
        }

        private bool Request(RequestAttribute attr, out string answer) {
            userInput.SendMessage(attr.RequestMessage);
            answer = userInput.RequestAnswer();

            if(string.Equals(answer, "exit")) {
                HandleExitRequest(attr);
            }

            return !string.IsNullOrWhiteSpace(answer);
        }

        private string AskRequired(RequestAttribute attr) {
            string output;

            while(!Request(attr, out output)) {
                userInput.SendMessage($"{attr.DisplayName} has to be assigned. If you want to quit to the start, write \"exit\".");
            }

            return output;
        }

        private void HandleExitRequest(RequestAttribute attr) {
            if(userInput.YesOrNo($"Do you want to return to the start? Y/N (refusing will set \"exit\" as the value of {attr.DisplayName})")) {
                throw new InputExitedException();
            }
        }

        private class InputExitedException : Exception {}
    }
}
