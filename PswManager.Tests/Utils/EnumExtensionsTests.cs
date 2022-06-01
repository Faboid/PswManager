using PswManager.Utils;
using Xunit;

namespace PswManager.Tests.Utils {
    public class EnumExtensionsTests {

        [Theory]
        [InlineData(TestEnum.One, TestEnum.One)]
        [InlineData(TestEnum.Two, TestEnum.Two)]
        [InlineData(TestEnum.Three, TestEnum.Three)]
        [InlineData(TestEnum.Four, TestEnum.Four)]
        public void EnumsAreEqual(TestEnum a, TestEnum b) => Assert.True(a.IsEqual(b));

        [Theory]
        [InlineData(TestEnum.Two, TestEnum.Three)]
        [InlineData(TestEnum.Two, TestEnum.One)]
        [InlineData(TestEnum.One, TestEnum.Four)]
        public void EnumsAreNotEqual(TestEnum a, TestEnum b) => Assert.False(a.IsEqual(b));


        public enum TestEnum {
            One = 10,
            Two = 20,
            Three = 30,
            Four = 40
        }

    }
}
