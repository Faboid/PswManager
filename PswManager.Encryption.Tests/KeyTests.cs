using PswManager.Encryption.Cryptography;
using Xunit;

namespace PswManager.Encryption.Tests;

public class KeyTests {

    [Theory]
    [InlineData("left", "right", false)]
    [InlineData("equal", "equal", true)]
    [InlineData("", "", true)]
    [InlineData(null, null, true)]
    [InlineData(null, "hello", false)]
    public void EqualityReturnsExpected(string? a, string? b, bool expected) {

        Key? left = a is not null ? new Key(a.ToCharArray()) : null;
        Key? right = b is not null ?  new Key(b.ToCharArray()) : null;
        Assert.Equal(expected, left == right);

    }

}