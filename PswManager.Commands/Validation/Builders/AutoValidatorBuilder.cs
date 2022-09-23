using PswManager.Commands.Validation.Attributes;
using PswManager.Commands.Validation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PswManager.Commands.Validation.Validators;
using PswManager.Extensions;

[assembly: InternalsVisibleTo("PswManager.Commands.Tests")]
namespace PswManager.Commands.Validation.Builders; 
public class AutoValidatorBuilder<TObj> {

    private readonly IReadOnlyList<PropertyInfo> properties;
    readonly IReadOnlyList<PropertyInfo> requiredProperties;
    readonly List<(ValidationRule validator, List<PropertyInfo> props)> customValidators = new();

    internal AutoValidatorBuilder() {
        properties = typeof(TObj).GetProperties().ToList();
        requiredProperties = properties.Where(x => x.GetCustomAttribute<RequiredAttribute>() != null).ToList();
    }

    public AutoValidatorBuilder<TObj> AddRule(ValidationRule validationLogic) {
        var props = properties.Where(x => x.GetCustomAttribute(validationLogic.GetAttributeType) != null).ToList();
        props.ForEach(x => ValidateProperty(x, validationLogic));
        customValidators.Add((validationLogic, props));
        return this;
    }

    public AutoValidatorBuilder<TObj> AddRule<TValidationRule>() where TValidationRule : ValidationRule, new() {
        var validationLogic = new TValidationRule();
        return AddRule(validationLogic);
    }

    /// <summary>
    /// Note: this overload is significantly slower than the others and has less type safety. 
    /// <br/>It's been implemented for learning purposes—if you require type-safety and performance, 
    /// use the other AddRule() overloads.
    /// </summary>
    /// <typeparam name="TValidationRule">The type of ValidationRule.</typeparam>
    /// <param name="args">The arguments of ANY constructor of the given <typeparamref name="TValidationRule"/> type. 
    /// Will throw an <see cref="ArgumentException"/> if there isn't an appropriate constructor.</param>
    /// <returns>This instance of AutoValidatorBuilder.</returns>
    /// <exception cref="ArgumentException">Thrown if the given <paramref name="args"/> do not respect any constructor of <typeparamref name="TValidationRule"/></exception>
    public AutoValidatorBuilder<TObj> AddRule<TValidationRule>(params object[] args) where TValidationRule : ValidationRule {
        var argTypes = args.Select(x => x.GetType()).ToArray();
        var constructor = typeof(TValidationRule).GetConstructor(argTypes);

        if(constructor == null) {
            throw new ArgumentException(
                $"The arguments given ({argTypes.Select(x => x.Name).JoinStrings(", ")}) to AddRule<TValidationRule>(params object[] args)" +
                $"do not respect any constructor of the given TValidationRule type({typeof(TValidationRule)}).", nameof(args));
        }

        var validationLogic = (ValidationRule)Activator.CreateInstance(typeof(TValidationRule), args);
        return AddRule(validationLogic);
    }

    public IAutoValidator<TObj> Build() {
        return new AutoValidator<TObj>(requiredProperties, customValidators);
    }

    /// <summary>
    /// This method makes sure the attributes are placed in the correct data types and will throw if they're not.
    /// </summary>
    private void ValidateProperty(PropertyInfo property, ValidationRule validationLogic) {

        if(property.PropertyType == validationLogic.GetDataType) {
            return;
        }

        throw new InvalidCastException(
            $"The object {nameof(TObj)} uses the attribute {validationLogic.GetAttributeType} for property {property.Name} of type {property.PropertyType}, " +
            $"but the ValidationRule given only supports the type {validationLogic.GetDataType}."
            );
    }

}
