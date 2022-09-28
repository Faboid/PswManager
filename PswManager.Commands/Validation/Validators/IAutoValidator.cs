using System.Collections.Generic;

namespace PswManager.Commands.Validation.Validators;
public interface IAutoValidator<T> {
    IEnumerable<string> Validate(T obj);
}
