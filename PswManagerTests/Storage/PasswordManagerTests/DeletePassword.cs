using PswManagerLibrary.Exceptions;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Storage.PasswordManagerTests {

    [Collection("TestHelperCollection")]
    public class DeletePassword {

        [Fact]
        public void DeletePasswordSuccess() {

            //arrange
            TestsHelper.SetUpDefault();
            var manager = TestsHelper.PswManager;
            string name = TestsHelper.DefaultValues.GetValue(1, DefaultValues.TypeValue.Name);
            bool exist;

            //act
            exist = manager.GetPassword(name) is string;
            manager.DeletePassword(name);

            //assert
            Assert.True(exist);
            Assert.Throws<InvalidCommandException>(() => manager.GetPassword(name));

        }

    }
}
