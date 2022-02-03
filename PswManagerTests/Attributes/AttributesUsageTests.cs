using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PswManagerCommands.Parsing.Attributes;
using Xunit.Abstractions;
using PswManagerCommands.Validation.Attributes;

namespace PswManagerTests.Attributes {
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
                .WhereHasAttributes<ParseableKeyAttribute, RequiredAttribute>();

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

        public static IEnumerable<PropertyInfo> WhereHasAttributes<TAttribute1, TAttribute2>(this IEnumerable<PropertyInfo> properties) 
            where TAttribute1 : Attribute where TAttribute2 : Attribute
            => properties.Where(x => x.HasAttributes<TAttribute1, TAttribute2>());

        public static bool HasAttributes<TAttribute1, TAttribute2>(this PropertyInfo properties) 
            where TAttribute1 : Attribute where TAttribute2 : Attribute
            => properties.GetCustomAttributes().Where(x => x is TAttribute1 or TAttribute2).Any();

    }
}
