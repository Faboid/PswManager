namespace PswManager.Commands.Validation.Models;
public sealed class IndexHelper {

    /// <summary>
    /// Creates an IndexHelper class.
    /// </summary>
    /// <param name="index">The index of this condition. No duplicates are allowed.</param>
    /// <param name="requiredSuccesses">The conditions required to check this one. Note that the condition "-1" is valid only if all automatic test pass.</param>
    public IndexHelper(ushort index, params int[] requiredSuccesses) {
        Index = index;
        RequiredSuccesses = requiredSuccesses;
    }

    internal IndexHelper(int index, params int[] requiredSuccesses) {
        Index = index;
        RequiredSuccesses = requiredSuccesses;
    }

    public int Index { get; private set; }

    public int[] RequiredSuccesses { get; private set; }

}
