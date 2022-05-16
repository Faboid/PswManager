using PswManager.Commands;
using PswManager.Commands.Unused.Parsing;
using PswManager.Commands.Unused.Parsing.Attributes;
using System.Collections.Generic;
using Xunit;

namespace PswManager.Tests.Parsing {
    public class ParserTests {

        [Fact]
        public void ParseSuccessfully() {
            
            //arrange
            IParser parser = Parser.CreateInstance();

            string separator = parser.Separator;
            char equal = parser.Equal;
            //as soon as the functionality gets added, put a separator within one of the values
            string name = "yoyo";
            string password = $"_ghigh riehg :/£$GG";
            string email = "email@here.com";
            string command = $"{separator}name{equal}{name}{separator}pass{equal}{password}{separator}ema{equal}{email}";

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
            yield return NewObj($"{separator}name{equal}nameValue{separator}fakeKey{equal}value{separator}ema{equal}yoyo");
            yield return NewObj($"{separator}name{equal}nameValue{separator}ema{equal}value{separator}ema{equal}yoyo");

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

    internal class CustomObject : ICommandInput {

        [ParseableKey("name")]
        public string Name { get; private set; }

        [ParseableKey("pass")]
        public string Password { get; private set; }

        [ParseableKey("ema")]
        public string Email { get; private set; }

    }

}
