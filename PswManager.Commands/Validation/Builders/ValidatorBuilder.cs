using PswManager.Commands.Validation.Models;
using PswManager.Commands.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PswManager.Tests")]
namespace PswManager.Commands.Validation.Builders;
public class ValidatorBuilder<T> {

    readonly List<ICondition<T>> conditions = new();
    readonly List<IAutoValidator<T>> autoValidators = new();

    internal ValidatorBuilder() { }

    public ValidatorBuilder<T> AddCondition(short index, Func<T, bool> conditionFunction, string errorMessage) {
        return AddCondition(new IndexHelper(index), conditionFunction, errorMessage);
    }

    public ValidatorBuilder<T> AddCondition(IndexHelper index, Func<T, bool> conditionFunction, string errorMessage) {
        conditions.Add(new Condition<T>(index, conditionFunction, errorMessage));
        return this;
    }

    public ValidatorBuilder<T> AddCondition(ICondition<T> condition) {
        conditions.Add(condition);
        return this;
    }

    public ValidatorBuilder<T> AddAutoValidator(IAutoValidator<T> autoValidator) {
        autoValidators.Add(autoValidator);
        return this;
    }

    public IValidator<T> Build() {
        return new Validator<T>(conditions, autoValidators);
    }

}

