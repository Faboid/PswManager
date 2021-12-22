﻿using PswManagerLibrary.OldCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Commands {
    public class CommandTests {

        [Fact]
        public void RetainPasswordCorrectly() {
            //todo - delete or change this. The test is outdated.

            //arrange
            Command command;
            string commandType = "psw";
            string argumentsCommand = " di21gh&& #@   - . Yfa %£$,";
            string fullcommand = $"{commandType} {argumentsCommand}";
            CommandType expMainCommand = CommandType.Psw;

            //act
            command = new Command(fullcommand);

            //assert
            Assert.Equal(command.MainCommand, expMainCommand);
            Assert.Equal(string.Join(' ', command.Arguments), argumentsCommand);

        }

    }
}
