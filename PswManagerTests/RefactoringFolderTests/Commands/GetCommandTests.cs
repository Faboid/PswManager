using PswManagerLibrary.RefactoringFolder;
using PswManagerLibrary.RefactoringFolder.Commands;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.RefactoringFolderTests.Commands {

    [Collection("TestHelperCollection")]
    public class GetCommandTests {

        readonly GetCommand getCommand;

        public GetCommandTests() {
            getCommand = new(TestsHelper.PswManager);
        }

        [Fact]
        public void GetCommand_Success() {

            //arrange
            CommandResult result;

            //act
            result = getCommand.Run(new string[] { TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name) });

            //assert
            Assert.Equal(TestsHelper.DefaultValues.values[0], result.QueryReturnValue);

        }


    }
}
