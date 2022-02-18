using PswManagerCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Commands.ArgsModels;
using PswManagerLibrary.Extensions;
using PswManagerTests.Commands.Helper;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Commands {

    [Collection("TestHelperCollection")]
    public class DeleteCommandTests {

        public DeleteCommandTests() {
            IDataFactory dataFactory = new DataFactory(TestsHelper.Paths);
            delCommand = new DeleteCommand(dataFactory.GetDataDeleter(), TestsHelper.AutoInput);
            dataHelper = dataFactory.GetDataHelper();
        }

        readonly IDataHelper dataHelper;
        readonly DeleteCommand delCommand;

        [Fact]
        public void DeleteSuccessfully() {

            //arrange
            TestsHelper.SetUpDefault();
            string name = TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name);
            var obj = ClassBuilder.Build(delCommand, new List<string> { name });

            //act
            bool exist = dataHelper.AccountExist(name);
            delCommand.Run(obj);

            //assert
            Assert.True(exist);
            Assert.False(dataHelper.AccountExist(name));

        }

        public static IEnumerable<object[]> ExpectedValidationFailuresData() {
            static object[] NewObj(string errorMessage, string name)
                => new object[] {
                    errorMessage,
                    ClassBuilder.Build(new DeleteCommand(null, null), new List<string> { name })
                };

            //string validName = TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name);

            //yield return new object[] { ValidationCollection.ArgumentsNullMessage, null };

            //yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { null } };
            //yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { "" } };
            //yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { "     " } };

            yield return NewObj(new ValidationCollection(null).InexistentAccountMessage(), "fakeAccountName");

            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, Array.Empty<string>() };
            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName, "eiwghrywhgi" } };
            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName, "tirhtewygh", "email@somewhere.com"} };
            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName, "tirhtewygh", "email@somewhere.com", "something"} };
        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
        public void ExpectedValidationFailures(string expectedErrorMessage, ICommandInput args) {

            //arrange
            bool valid;
            CommandResult result;

            //act
            valid = delCommand.Validate(args).success;
            result = delCommand.Run(args);

            //assert
            Assert.False(valid);
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
            Assert.Contains(expectedErrorMessage, result.ErrorMessages);

        }

    }
}
