using System.Collections.Generic;

namespace PswManagerCommands.Validation.Validators {
    public interface IValidator<T> {
        IEnumerable<string> Validate(T obj);
    }
}
