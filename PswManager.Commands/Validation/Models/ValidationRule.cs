using PswManager.Commands.Validation.Attributes;
using System;

namespace PswManager.Commands.Validation.Models; 
public abstract class ValidationRule {

    public Type GetAttributeType;
    public Type GetDataType;

    protected ValidationRule(Type attributeType, Type dataType) {
        if(!attributeType.IsSubclassOf(typeof(RuleAttribute))) {
            throw new InvalidCastException(nameof(attributeType));
        }

        GetAttributeType = attributeType;
        GetDataType = dataType;
    }

    public bool Validate(RuleAttribute attribute, object value) {
        if(GetAttributeType != attribute.GetType()) {
            throw new ArgumentException($"The given attribute is of the type {attribute.GetType()}, but this ValidationRule supports only {GetAttributeType}.", nameof(attribute));
        }
        if(value is not null && GetDataType != value.GetType()) {
            throw new ArgumentException($"The given value is of the type {value.GetType()}, but this ValidationRule supports only {GetDataType}.", nameof(value));
        }

        //As this is only for validation, the exceptions thrown from here shouldn't be fatal.
        //InnerLogic should NOT have any logic outside of validation.
        try {
            return InnerLogic(attribute, value);
        }
        catch {
            return false;
        }
    }

    protected abstract bool InnerLogic(RuleAttribute attribute, object value);

}

