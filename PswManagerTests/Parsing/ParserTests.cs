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
            string name = "yoyo";
            string password = $"_ghigh riehg :/£$GG";
            string email = "email@here.com";
            string command = $"{separator}n{equal}{name}{separator}pass{equal}{password}{separator}ema{equal}{email}";

            //act
            var result = parser.Setup<CustomObject>().Parse(command);
            var obj = result.Object as CustomObject;

            //assert
            Assert.Equal(ParsingResult.Success.Success, result.Result);
            Assert.Equal(name, obj.Name);
            Assert.Equal(password, obj.Password);
            Assert.Equal(email, obj.Email);

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
