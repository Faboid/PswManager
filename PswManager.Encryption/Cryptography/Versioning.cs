using System.Text;

namespace PswManager.Encryption.Cryptography;

/// <summary>
/// Handles versioning by combining a unique identifier to a number of iterations.
/// </summary>
internal class Versioning {

    private string _currentVersion { get; init; }
    private const char openingChar = '(';
    private const char closingChar = ')';

    /// <summary>
    /// Instantiates an instance of <see cref="Versioning"/> with the given <paramref name="currVersion"/>.
    /// </summary>
    /// <remarks>
    /// Throws <see cref="NotSupportedException"/> if the given <paramref name="currVersion"/> isn't implemented.
    /// </remarks>
    /// <param name="currVersion"></param>
    /// <exception cref="NotSupportedException"></exception>
    public Versioning(string currVersion) {
        //checks if the given version exists
        _ = GetRfcDerivedBytesIterations(currVersion);
        _currentVersion = currVersion;
    }

    /// <summary>
    /// </summary>
    /// <returns>The current version.</returns>
    public string GetVersion() {
        return _currentVersion;
    }

    /// <summary>
    /// Adds the version text to the beginning of an array of bytes.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public byte[] AddVersion(byte[] text) {
        byte[] versionAsBytes = Encoding.Unicode.GetBytes($"{openingChar}{GetVersion()}{closingChar}");
        return versionAsBytes.Concat(text).ToArray();
    }

    /// <summary>
    /// Converts a <paramref name="version"/> identifier to its iteration numbers.
    /// </summary>
    /// <remarks>
    /// Throws <see cref="NotSupportedException"/> if the given version isn't supported yet.
    /// </remarks>
    /// <param name="version"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static int GetRfcDerivedBytesIterations(string version) => version switch {
        "test.1" => 1000,
        "test.2" => 20000,
        "1.00" => 500000,
        _ => throw new NotSupportedException($"The given version isn't supported yet: {version}")
    };

    /// <summary>
    /// Extracts the version identifier from a string of text. <paramref name="text"/> will be left as the version-less encrypted string.
    /// </summary>
    /// <remarks>
    /// <see cref="ArgumentException"/> will be thrown if the given <paramref name="text"/> isn't properly formatted.
    /// </remarks>
    /// <param name="text"></param>
    /// <returns>The version identifier.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static string ExtractVersion(ref string text) {
        var bytes = Convert.FromBase64String(text).ToList();
        int openIndex = bytes.FindIndex(x => x == (byte)openingChar);
        int closeIndex = bytes.FindIndex(x => x == (byte)closingChar) + 1; //plus one because the bytes are saved as [value], [extra 0] for each char

        if(openIndex == -1) {
            throw new ArgumentException("The cipher text doesn't have the opening bracket for the version.");
        }
        if(closeIndex == -1) {
            throw new ArgumentException("The cipher text doesn't have the closing bracket for the version.");
        }

        string version = Encoding.Unicode.GetString(bytes.Skip(openIndex).Take(closeIndex + 1).ToArray());
        version = version.TrimStart(openingChar).TrimEnd(closingChar);
        text = Convert.ToBase64String(bytes.Skip(openIndex + closeIndex + 1).ToArray());

        return version;
    }

}

