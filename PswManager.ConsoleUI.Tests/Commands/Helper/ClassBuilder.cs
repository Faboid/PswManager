using PswManager.Commands;
using PswManager.ConsoleUI.Attributes;
using System.Reflection;

namespace PswManager.ConsoleUI.Tests.Commands.Helper {
    internal class ClassBuilder {

        //note: this will crash if given a TCommand whose parent doesn't implement a generic.
        //It's meant to be used with children that inherit from BaseCommand
        public static ICommandInput Build<TCommand>(IEnumerable<string> args) where TCommand : ICommand {
            try {

                return Build(typeof(TCommand).BaseType!.GetGenericArguments()[0], args);
            } catch(IndexOutOfRangeException) {

                throw ExceptionsFactory.CreateInvCastException<TCommand>("this builder", "ClassBuilder.Build()");
            }
        }

        public static ICommandInput Build(in ICommand command, IEnumerable<string> args) {
            return Build(command.GetCommandInputType, args);
        }

        private static ICommandInput Build(Type type, IEnumerable<string> args) {

            ICommandInput output = (ICommandInput)Activator.CreateInstance(type)!;

            var props = type
                .GetProperties()
                .Where(x => x.GetCustomAttribute<RequestAttribute>() != null)
                .OrderByDescending(x => x.Name);

            var zip = props.Zip(args, (p, a) => new { Prop = p, Arg = a });
            foreach(var item in zip) {
                item.Prop.SetValue(output, item.Arg);
            }

            return output;
        }

    }
}
