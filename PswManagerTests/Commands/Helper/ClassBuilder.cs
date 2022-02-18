using PswManagerCommands;
using PswManagerLibrary.UIConnection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerTests.Commands.Helper {
    internal class ClassBuilder {

        public static ICommandInput Build(in ICommand command, List<string> args) {
            return Build(command.GetCommandInputType, args);
        }

        private static ICommandInput Build(Type type, List<string> args) {

            ICommandInput output = (ICommandInput)Activator.CreateInstance(type);

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
