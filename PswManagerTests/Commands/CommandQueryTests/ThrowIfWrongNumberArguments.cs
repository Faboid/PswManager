using PswManagerLibrary.Commands;
using PswManagerLibrary.Exceptions;
using PswManagerTests.TestsHelpers;
using PswManagerTests.TestsHelpers.MockPasswordManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Commands.CommandQueryTests {
    
    [Collection("TestHelperCollection")]
    public class ThrowIfWrongNumberArguments {
    
        [Theory]
        [InlineData("psw first second third")]
        [InlineData("create first second third fourth")]
        [InlineData("get first second")]
        [InlineData("edit first")]
        [InlineData("edit first second third fourth fifth sixth")]
        [InlineData("delete first second third fourth fifth")]
        public void Throw_IncorrectArgumentsNumber(string command) {

            //arrange
            CommandQuery query = new CommandQuery(TestsHelper.paths, TestsHelper.autoInput, new EmptyPasswordManager());

            //act

            //assert
            Assert.Throws<InvalidCommandException>(() => query.Start(command));

        }

    }
}
