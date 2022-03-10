using System.Collections.Generic;

namespace PswManagerCommands.Validation {
    public interface IValidator<T> {
        IEnumerable<string> Validate(T obj);
    }
}
