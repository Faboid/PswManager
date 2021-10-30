using PswManagerLibrary.Commands;
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
        public void HashTableIsCorrect(string input, CommandType expectedType) {

            //arrange
            CommandType result;

            //act
            result = CommandTypeAnalyzer.Get(input);

            //assert
            Assert.Equal(expectedType, result);

        }

    }
}
