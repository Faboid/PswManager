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
            IParser parser = new Parser();

            string separator = parser.Separator;
            string name = "yoyo";
            string password = "_ghigh riehg/£$GG";
            string email = "email@here.com";
            string command = $"{separator}n:{name}{separator}pass:{password}{separator}ema:{email}";

            //act
            var success = parser.TryParse(command, out CustomObject obj);

            //assert
            Assert.True(success);
            Assert.Equal(name, obj.Name);
            Assert.Equal(password, obj.Password);
            Assert.Equal(email, obj.Email);

        }

    }

    internal class CustomObject : IParseable {

        public string Name { get; private set; }
        public string Password { get; private set; }
        public string Email { get; private set; }

        public void RegisterAll(IParser parser) {
            VariableReference<string> nameRef = new(() => Name, (value) => Name = value);
            VariableReference<string> passwordRef = new(() => Password, (value) => Password = value);
            VariableReference<string> emailRef = new(() => Email, (value) => Email = value);

            parser.Register("n", nameRef);
            parser.Register("pass", passwordRef);
            parser.Register("ema", emailRef);
        }
    }

}
