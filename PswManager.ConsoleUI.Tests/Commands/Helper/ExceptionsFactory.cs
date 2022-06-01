namespace PswManager.ConsoleUI.Tests.Commands.Helper {
    internal class ExceptionsFactory {

        public static ArgumentException CreateArgException(string propertyName) {
            return new($"{propertyName} doesn't exist. Was the command's args' class modified recently?", nameof(propertyName));
        }

        public static InvalidCastException CreateInvCastException<TCommand>(string sourceClass, string swapWithMethod) {
            return new($"The given type is not usable by {sourceClass}. " +
                    $"Please use the non-generic version of {swapWithMethod}. " +
                    $"{Environment.NewLine}Given Type: {typeof(TCommand).FullName}");
        }

    }
}
