using PswManagerLibrary.Commands;
using PswManagerLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Commands {
    public class CommandTypeAnalyzerTests {

        [Theory]
        [InlineData("psw", CommandType.Psw)]
        [InlineData("get", CommandType.Get)]
        [InlineData("create", CommandType.Create)]
        [InlineData("edit", CommandType.Edit)]
        [InlineData("delete", CommandType.Delete)]
        public void IsCorrect(string input, CommandType expectedType) {

            //arrange
            CommandType result;

            //act
            result = CommandTypeAnalyzer.Get(input);

            //assert
            Assert.Equal(expectedType, result);

        }

        [Fact]
        public void InvalidInputGivesCorrectException() {

            //arrange
            string nonsensicalInput = "justsomerandomthingthatcannotbevalid";

            //act

            //assert
            Assert.Throws<InvalidCommandException>(() => CommandTypeAnalyzer.Get(nonsensicalInput));

        }

    }
}
