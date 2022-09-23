using PswManager.Async.Locks;
using Xunit;

namespace PswManager.Async.Tests.Locks; 
public class LockResultTests {

    [Fact]
    public void FailureIsOppositeOfSuccess() {

        //arrange
        LockResult successResult = new(true);
        LockResult failureResult = new(false);

        //assert
        Assert.True(successResult.Success);
        Assert.False(successResult.Failed);

        Assert.False(failureResult.Success);
        Assert.True(failureResult.Failed);

    }

    [Fact]
    public void InstanceWithValueCorrectly() {

        //arrange
        int value = 5;
        LockResult<int> withValueResult = new(value);
        LockResult<int> failedResult = new("This has failed");

        //assert
        Assert.True(withValueResult.Success);
        Assert.False(withValueResult.Failed);
        Assert.Equal(value, withValueResult.Value);

        Assert.False(failedResult.Success);
        Assert.True(failedResult.Failed);
        Assert.Equal(default, failedResult.Value);

    }

}
