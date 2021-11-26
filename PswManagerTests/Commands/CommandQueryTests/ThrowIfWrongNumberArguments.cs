using PswManagerLibrary.Commands;
using PswManagerLibrary.Exceptions;
using PswManagerTests.TestsHelpers;
using PswManagerTests.TestsHelpers.MockFactories;
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

        readonly CommandQuery query;

        public ThrowIfWrongNumberArguments() {

            query = new CommandQuery(TestsHelper.Paths, TestsHelper.AutoInput, new EmptyPasswordManagerFactory());
            query.Start(new Command("psw pswpassword emapassword"));
        }
    
        [Theory]
        [InlineData("psw first second third")]
        [InlineData("create first second third fourth")]
        [InlineData("get first second")]
        [InlineData("edit first")]
        [InlineData("edit first second third fourth fifth sixth")]
        [InlineData("delete first second third fourth fifth")]
        public void Throw_IncorrectArgumentsNumber(string command) {

            //assert
            Assert.Throws<InvalidCommandException>(() => query.Start(command));

        }

        [Theory]
        [InlineData("psw first second")]
        [InlineData("create first second third")]
        [InlineData("get first")]
        [InlineData("edit first second")]
        [InlineData("edit first second third")]
        [InlineData("edit first second third fourth")]
        [InlineData("delete first")]
        public void DoNotThrow_CorrectArgumentsNumber(string command) {

            //arrange

            //act
            var exception = Record.Exception(() => query.Start(command));

            //assert
            Assert.Null(exception);
        }

    }
}
