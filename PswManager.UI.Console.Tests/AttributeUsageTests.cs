using PswManager.Commands.Validation.Attributes;
using System.Reflection;
using Xunit.Abstractions;

namespace PswManager.UI.Console.Tests;
public class AttributesUsageTests {

    public AttributesUsageTests(ITestOutputHelper output) {
        this.output = output;
    }

    readonly ITestOutputHelper output;

    [Fact]
    public void UsedOnStringsOnly() {
        //this test is a refactored version of Sel's answer in https://stackoverflow.com/questions/8382536/allow-a-custom-attribute-only-on-specific-type/40871170

        var propsWithFaultyUsage = AttributesUsageTestsHelper
            .GetAllClasses()
            .GetAllProperties()
            .WhereIsNotString()
            .WhereHasAttribute<RequiredAttribute>();

        var propsErrorLocations = propsWithFaultyUsage.Select(FormatMessage);

        foreach(var error in propsErrorLocations) {
            output.WriteLine(error);
        }

        Assert.Empty(propsErrorLocations);
    }

    private static string FormatMessage(PropertyInfo x)
        => $"Property '{x.DeclaringType}.{x.Name}' has invalid type: '{x.PropertyType}'. The only allowed type when using this attribute is 'string'.";

}

internal static class AttributesUsageTestsHelper {

    public static IEnumerable<Type> GetAllClasses()
        => AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());

    public static IEnumerable<PropertyInfo> GetAllProperties(this IEnumerable<Type> classes)
        => classes.SelectMany(x => x.GetProperties());

    public static IEnumerable<Attribute> GetAttributes(this PropertyInfo properties)
        => properties.GetCustomAttributes();

    public static IEnumerable<PropertyInfo> WhereIsNotString(this IEnumerable<PropertyInfo> properties)
        => properties.Where(x => x.PropertyType != typeof(string));

    public static IEnumerable<PropertyInfo> WhereHasAttribute<TAttribute1>(this IEnumerable<PropertyInfo> properties)
        where TAttribute1 : Attribute
        => properties.Where(x => x.HasAttribute<TAttribute1>());

    public static bool HasAttribute<TAttribute1>(this PropertyInfo properties)
        where TAttribute1 : Attribute
        => properties.GetCustomAttributes().Any(x => x is TAttribute1);

}
