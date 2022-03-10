using System.Collections.Generic;

namespace PswManagerCommands.Validation {
    public interface IAutoValidator<T> {
        IEnumerable<string> Validate(T obj);
    }
}
