using PswManagerCommands;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Storage;
using PswManagerTests.TestsHelpers;
using Xunit;
using Xunit.Abstractions;

namespace PswManagerTests.Commands {
    [Collection("TestHelperCollection")]
    public class EditCommandTests : BaseCommandTests {

        public EditCommandTests(ITestOutputHelper output) : base(output) {
            pswManager = TestsHelper.PswManager;
            editCommand = new EditCommand(pswManager);
            TestsHelper.SetUpDefault();
        }

        readonly IPasswordManager pswManager;
        readonly ICommand editCommand;

        protected override CommandTestsHelper GetHelper() {
            return new CommandTestsHelper(
                (EditCommand)editCommand,
                new string[] { "fakeAccountValue", "name:randomValue" },
                2, 4
            );
        }

        //todo - implement tests
    }
}
