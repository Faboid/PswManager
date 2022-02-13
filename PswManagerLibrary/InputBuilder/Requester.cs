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
    public class Requester<T> where T: new() {

        private readonly IReadOnlyList<(PropertyInfo prop, string message)> required;
        private readonly IReadOnlyList<(PropertyInfo prop, string message)> optional;
        readonly private IUserInput userInput;

        public Requester(IUserInput userInput) {
            this.userInput = userInput;
            
            var props = typeof(T).GetProperties();

            required = props
                .Select(prop => (prop, prop.GetCustomAttribute<RequestAttribute>()))
                .Where(x => x.Item2 != null)
                .Select(x => (x.prop, x.Item2.RequestMessage))
                .ToList();

            optional = props
                .Select(prop => (prop, prop.GetCustomAttribute<OptionalAttribute>()))
                .Where(x => x.Item2 != null)
                .Select(x => (x.prop, x.Item2.RequestMessage))
                .ToList();
        }

        public bool Build(out T result) {
            var output = new T();

            required
                .Select(x => (x.prop, AskRequired(x.message)))
                .ForEach(x => x.prop.SetValue(output, x.Item2));

            optional
                .Select(x => (x.prop, Request(x.message, out string answer), answer))
                .Where(x => x.Item2)
                .ForEach(x => x.prop.SetValue(output, x.answer));


            if(required.Any(x => string.IsNullOrWhiteSpace((string)x.prop.GetValue(output)))) {
                result = default;
                return false;
            }

            result = output;
            return true;
        }

        private bool Request(string message, out string answer) {
            userInput.SendMessage(message);
            answer = userInput.RequestAnswer();

            return !string.IsNullOrWhiteSpace(answer);
        }

        private string AskRequired(string message) {
            string output;

            while(!Request(message, out output)) {
                userInput.SendMessage("This value has to be assigned.");
            }

            return output;
        }

    }
}
