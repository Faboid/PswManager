using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xunit;

namespace PswManagerTests {
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
