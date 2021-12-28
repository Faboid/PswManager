using System.IO;
using Xunit;

namespace PswManagerTests.TestsHelpers {
    public class TestsPathsTests {
    
        [Fact]
        public void FoundPathIsNonExistentAndValid() {

            //arrange
            string path;

            //act
            path = TestsPaths.GetNonExistentFolderPath();

            //assert
            Assert.True(Path.IsPathRooted(path));
            Assert.False(Directory.Exists(path));

        }

    }
}
