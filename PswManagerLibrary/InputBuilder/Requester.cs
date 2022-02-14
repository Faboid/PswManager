﻿using PswManagerHelperMethods;
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

        private readonly IReadOnlyList<(PropertyInfo prop, string message)> required;
        private readonly IReadOnlyList<(PropertyInfo prop, string message)> optional;
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
                .Select(x => (x.prop, x.Item2.RequestMessage))
                .ToList();

            optional = props
                .Where(x => x.Item2.Optional == true)
                .Select(x => (x.prop, x.Item2.RequestMessage))
                .ToList();
        }

        public bool Build(out object result) {
            var output = Activator.CreateInstance(type);

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

            required.Concat(optional)
                .ForEach(x => userInput.SendMessage($"{x.prop.Name}: {x.prop.GetValue(output) ?? "N/A" }"));

            return userInput.YesOrNo("Are these values correct?");
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
