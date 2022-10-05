using PswManager.Commands.Validation.Attributes;
using System;

namespace PswManager.Commands.Validation.Models;

/// <summary>
/// Provides an base rule from which to build validation logic.
/// </summary>
public abstract class ValidationRule {

    /// <summary>
    /// The type of the attribute linked to this rule.
    /// </summary>
    public Type GetAttributeType;
    
    /// <summary>
    /// The data type of the property/field the linked attribute is used on.
    /// </summary>
    public Type GetDataType;

    /// <summary>
    /// Initializes a <see cref="ValidationRule"/> and ensures the type of the attribute is a subclass of <see cref="RuleAttribute"/>.
    /// </summary>
    /// <param name="attributeType"></param>
    /// <param name="dataType"></param>
    /// <exception cref="InvalidCastException"></exception>
    protected ValidationRule(Type attributeType, Type dataType) {
        if(!attributeType.IsSubclassOf(typeof(RuleAttribute))) {
            throw new InvalidCastException(nameof(attributeType));
        }

        GetAttributeType = attributeType;
        GetDataType = dataType;
    }

    /// <summary>
    /// Returns whether the object respects the rule. 
    /// </summary>
    /// <remarks>
    /// Ensures that <paramref name="value"/> is of type <see cref="GetDataType"/> and <paramref name="attribute"/> is of type <see cref="GetAttributeType"/>. 
    /// Throws <see cref="ArgumentException"/> if they aren't.
    /// </remarks> 
    /// <param name="attribute"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
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
        } catch {
            return false;
        }
    }

    /// <summary>
    /// The logic of this rule.
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected abstract bool InnerLogic(RuleAttribute attribute, object value);

}

