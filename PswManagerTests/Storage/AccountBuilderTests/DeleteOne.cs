using PswManagerLibrary.Storage;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Storage.AccountBuilderTests {

    [Collection("TestHelperCollection")]
    public class DeleteOne {

        [Fact]
        public void DeleteOneCorrectly() {

            //arrange
            TestsHelper.SetUpDefault();
            AccountBuilder builder = new AccountBuilder(TestsHelper.Paths);
            string name = TestsHelper.DefaultValues.GetValue(1, DefaultValues.TypeValue.Name);
            bool exists;

            //act
            exists = builder.Search(name) is not null;
            builder.DeleteOne(name);

            //assert
            Assert.True(exists);
            Assert.Null(builder.Search(name));

        }

    }
}
