using PswManagerCommands;
using PswManagerCommands.Validation.Attributes;
using System;
using System.Reflection;

namespace PswManagerTests.Commands.Helper {
    public static class ErrorReader {

        public static string GetRequiredError<TCommand>(string propertyName) where TCommand : ICommand {
            var prop = propertyName
                .GetProperty<TCommand>();

            return prop
                .GetCustomAttribute<RequiredAttribute>()
                .GetErrorMessage(prop);
        }

        public static string GetError<TCommand, TAttribute>(string propertyName) 
            where TCommand : ICommand
            where TAttribute : RuleAttribute {

            return propertyName
                .GetProperty<TCommand>()
                .GetCustomAttribute<TAttribute>()
                .ErrorMessage;
        }

        private static PropertyInfo GetProperty<TCommand>(this string propertyName) {
            var prop = typeof(TCommand).BaseType //get parent object
                .GetGenericArguments()[0] //get the generic given to the parent object; the ICommandInput type
                .GetProperty(propertyName); //get the specific property

            if(prop == null) {
                throw new ArgumentException("The given property name doesn't exist. Was the command's args' class modified recently?", nameof(propertyName));
            }

            return prop;
        }

    }
}
