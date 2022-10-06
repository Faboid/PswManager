namespace PswManager.Encryption.Random;

/// <summary>
/// Provides a way to generate a consistent series of numbers.
/// </summary>
/// <remarks>
/// This class is all but random. Two instances with the same seed will return the same series of numbers.
/// Don't use it when randomness is important.
/// </remarks>
internal class SaltRandom {

    readonly int seed;
    IntRange current;

    /// <summary>
    /// Instances a <see cref="SaltRandom"/> with the given seed, and a constant amount of min and max range.
    /// </summary>
    /// <param name="seed"></param>
    public SaltRandom(int seed) : this(34, 64, seed) { }

    /// <summary>
    /// Instances <see cref="SaltRandom"/> with the given seed, max, and min values.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="seed"></param>
    public SaltRandom(int min, int max, int seed) {
        this.seed = seed;
        current = new IntRange(min, max, seed);
    }

    /// <summary>
    /// Returns the next value.
    /// </summary>
    /// <returns></returns>
    public int Next() {
        current += seed;
        return current;
    }

}
