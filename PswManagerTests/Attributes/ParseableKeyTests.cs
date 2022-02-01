using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PswManagerCommands.Parsing.Attributes;
using Xunit.Abstractions;

namespace PswManagerTests.Attributes {
    public class ParseableKeyTests {

        public ParseableKeyTests(ITestOutputHelper output) {
            this.output = output;
        }

        readonly ITestOutputHelper output;

        [Fact]
        public void UsedOnStringsOnly() {
            //this test is a refactored version of Sel's answer in https://stackoverflow.com/questions/8382536/allow-a-custom-attribute-only-on-specific-type/40871170

            var propsWithFaultyUsage = ParseableKeyTestsHelper
                .GetAllClasses()
                .GetAllProperties()
                .WhereIsNotString()
                .WhereHasAttribute<ParseableKeyAttribute>();

            var propsErrorLocations = propsWithFaultyUsage.Select(FormatMessage);

            foreach(var error in propsErrorLocations) { 
                output.WriteLine(error);
            }

            Assert.Empty(propsErrorLocations);
        }

        private static string FormatMessage(PropertyInfo x) => $"Property '{x.DeclaringType}.{x.Name}' has invalid type: '{x.PropertyType}'. The only allowed type is 'string'.";

    }

    internal static class ParseableKeyTestsHelper {

        public static IEnumerable<Type> GetAllClasses() 
            => AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());

        public static IEnumerable<PropertyInfo> GetAllProperties(this IEnumerable<Type> classes) 
            => classes.SelectMany(x => x.GetProperties());

        public static IEnumerable<PropertyInfo> WhereHasAttribute<TAttribute>(this IEnumerable<PropertyInfo> properties) where TAttribute : Attribute
            => properties.Where(x => x.GetCustomAttribute<TAttribute>() != null);

        public static IEnumerable<PropertyInfo> WhereIsNotString(this IEnumerable<PropertyInfo> properties) 
            => properties.Where(x => x.PropertyType != typeof(string));

    }
}
