using System.Collections.Generic;

namespace PswManager.Commands.Validation.Validators; 
public interface IValidator<T> {
    IEnumerable<string> Validate(T obj);
}
