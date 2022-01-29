using PswManagerCommands.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Parsing {
    public class ParserTests {

        [Fact]
        public void ParseSuccessfully() {
            
            //arrange
            IParser parser = Parser.CreateInstance();

            string separator = parser.Separator;
            char equal = parser.Equal;
            //todo - as soon as the functionality gets added, put a separator within one of the values
            string name = "yoyo";
            string password = $"_ghigh riehg :/£$GG";
            string email = "email@here.com";
            string command = $"{separator}n{equal}{name}{separator}pass{equal}{password}{separator}ema{equal}{email}";

            //act
            var result = parser.Setup<CustomObject>().Parse(command);
            var obj = result.Object as CustomObject;

            //assert
            Assert.Null(result.ErrorMessage);
            Assert.Equal(ParsingResult.Success.Success, result.Result);
            Assert.Equal(name, obj.Name);
            Assert.Equal(password, obj.Password);
            Assert.Equal(email, obj.Email);

        }

        public static IEnumerable<object[]> ParseFailureData() {
            static object[] NewObj(string command) => new object[] { command };
            IParser parser = Parser.CreateInstance();
            string separator = parser.Separator;
            char equal = parser.Equal;

            yield return NewObj("tibhtuhwygirh");
            yield return NewObj($"{separator}n{equal}nameValue{separator}fakeKey{equal}value{separator}ema{equal}yoyo");
            yield return NewObj($"{separator}n{equal}nameValue{separator}ema{equal}value{separator}ema{equal}yoyo");

        }

        [Theory]
        [MemberData(nameof(ParseFailureData))]
        public void ParseFailure(string command) {

            //arrange
            IParser parser = Parser.CreateInstance();

            //act
            var result = parser.Setup<CustomObject>().Parse(command);

            //assert
            Assert.NotNull(result.ErrorMessage);
            Assert.Equal(ParsingResult.Success.Failure, result.Result);

        }

    }

    internal class CustomObject : IParseable {

        public string Name { get; private set; }
        public string Password { get; private set; }
        public string Email { get; private set; }

        public void AddReferences(Dictionary<string, Action<string>> dictionary) {
            dictionary.Add("n", (value) => Name = value);
            dictionary.Add("pass", (value) => Password = value);
            dictionary.Add("ema", (value) => Email = value);
        }

    }

}
