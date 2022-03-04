using PswManagerCommands;
using PswManagerCommands.Validation.Attributes;
using System;
using System.Reflection;

namespace PswManagerTests.Commands.Helper {
    public static class ErrorReader {

        public static string GetRequiredError(this ICommand cmd, string propertyName) {
            var prop = cmd.GetCommandInputType.GetProperty(propertyName);
            return prop.GetCustomAttribute<RequiredAttribute>().GetErrorMessage(prop);
        }

        public static string GetError<TAttribute>(this ICommand cmd, string propertyName) where TAttribute : RuleAttribute {
            return cmd.GetCommandInputType.GetProperty(propertyName).GetCustomAttribute<TAttribute>().ErrorMessage;
        }

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
            try {

                var prop = typeof(TCommand).BaseType //get parent object
                    .GetGenericArguments()[0] //get the generic given to the parent object; the ICommandInput type
                    .GetProperty(propertyName); //get the specific property

                if(prop == null) {
                    throw ExceptionsFactory.CreateArgException(propertyName);
                }

                return prop;

            } catch(IndexOutOfRangeException) {

                throw ExceptionsFactory.CreateInvCastException<TCommand>("the error reader", "ErrorReader.GetErrorMessage() or ErrorReader.GetRequiredErrorMessage()");
            }
        }

    }
}
