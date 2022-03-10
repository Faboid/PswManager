using System.Collections.Generic;

namespace PswManagerCommands.Validation.Validators {
    public interface IAutoValidator<T> {
        IEnumerable<string> Validate(T obj);
    }
}
