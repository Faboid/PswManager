﻿using PswManagerLibrary.Exceptions;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Storage.PasswordManagerTests {

    [Collection("TestHelperCollection")]
    public class EditPassword {

        public static IEnumerable<object[]> UpdatePasswordSuccessData() {
            var def = TestsHelper.DefaultValues;

            yield return new object[] {
                def.GetValue(1, DefaultValues.TypeValue.Name),
                def.GetValue(1, DefaultValues.TypeValue.Name),
                new[] { "password:newPassword1", "email:newEmail1" },
                $"{def.GetValue(1, DefaultValues.TypeValue.Name)} newPassword1 newEmail1"
            };
            yield return new object[] {
                def.GetValue(2, DefaultValues.TypeValue.Name),
                "newName2",
                new[] { "name:newName2", "email:newEmail2" },
                $"newName2 {def.GetValue(2, DefaultValues.TypeValue.Password)} newEmail2"
            };
            yield return new object[] {
                def.GetValue(3, DefaultValues.TypeValue.Name),
                "fixedName",
                new[] { "name:fixedName", "password:passfix", "email:fixed@email.com" },
                "fixedName passfix fixed@email.com"
            };
        }

        [Theory]
        [MemberData(nameof(UpdatePasswordSuccessData))]
        public void UpdatePasswordSuccess(string name, string newName, string[] args, string expected) {

            //arrange
            TestsHelper.SetUpDefault();
            var manager = TestsHelper.PswManager;
            string actual;

            //act
            manager.EditPassword(name, args);
            actual = manager.GetPassword(newName);

            //assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void UpdatePasswordFailure_NameDontExist() {

            //arrange
            TestsHelper.SetUpDefault();
            var manager = TestsHelper.PswManager;
            string name = "ThisnameDoesn'tExist";
            string[] arguments = new[] {
                "name:somerandomName", "password:heyhey"
            };

            bool exists;
            string errorMessage = "The given account doesn't exist.";

            //act
            var exception = Record.Exception(() => manager.EditPassword(name, arguments));
            exists = manager.AccountExist(name);

            //assert
            Assert.False(exists);
            Assert.IsType<InvalidCommandException>(exception);
            Assert.Equal(errorMessage, exception.Message);

        }

    }
}