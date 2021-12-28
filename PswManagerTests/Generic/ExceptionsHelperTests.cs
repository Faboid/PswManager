using PswManagerLibrary.Exceptions;
using PswManagerLibrary.Generic;
using System;
using Xunit;

namespace PswManagerTests.Generic {
    public class ExceptionsHelperTests {
    
        [Fact]
        public void IfTrue_ExceptionShouldBeThrown() {

            //arrange
            string message = "message";
            Exception ex = new InvalidCommandException(message);
            bool isTrue = true;

            //act

            //assert
            Assert.Throws<InvalidCommandException>(() => isTrue.IfTrueThrow(ex));

        }

        [Fact]
        public void IfTrue_ExceptionShouldNotBeThrown() {

            //arrange
            string message = "message";
            Exception ex = new InvalidCommandException(message);
            bool isFalse = false;

            //act
            var result = Record.Exception(() => isFalse.IfTrueThrow(ex));

            //assert
            Assert.Null(result);

        }

        [Fact]
        public void IfFalse_ExceptionShouldBeThrown() {

            //arrange
            string message = "message";
            Exception ex = new InvalidCommandException(message);
            bool isFalse = false;

            //act

            //assert
            Assert.Throws<InvalidCommandException>(() => isFalse.IfFalseThrow(ex));

        }

        [Fact]
        public void IfFalse_ExceptionShouldNotBeThrown() {

            //arrange
            string message = "message";
            Exception ex = new InvalidCommandException(message);
            bool isTrue = true;

            //act
            var result = Record.Exception(() => isTrue.IfFalseThrow(ex));

            //assert
            Assert.Null(result);

        }

    }
}
