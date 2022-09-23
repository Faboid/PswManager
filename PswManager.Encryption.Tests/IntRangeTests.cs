using PswManager.Encryption.Random;
using Xunit;

namespace PswManager.Encryption.Tests; 
public class IntRangeTests {

    [Theory]
    [InlineData(0, 10, 12, 2)]
    [InlineData(-50, 50, 130, -20)]
    public void CorrectValue(int min, int max, int toAdd, int expectedValue) {

        //arrange
        var range = new IntRange(min, max);

        //act
        range += toAdd;

        //assert
        Assert.Equal(expectedValue, range);

    }

    [Fact]
    public void DoesNotLeaveTheRange() {

        //arrange
        int min = -10;
        int max = 30;
        var range = new IntRange(min, max);

        //act
        range += max * 2;

        //assert
        Assert.True(range < max);

    }

    [Fact]
    public void ThrowIfMinLessThanMax() {

        int min = 50;
        int max = -50;

        Assert.Throws<ArgumentOutOfRangeException>(() => new IntRange(min, max));

    }

}
