using System.Text;

namespace PswManager.Encryption.Cryptography; 
internal class Versioning {

    private string CurrentVersion { get; init; }
    private const char openingChar = '(';
    private const char closingChar = ')';

    public Versioning(string currVersion) {
        //checks if the given version exists
        _ = GetRfcDerivedBytesIterations(currVersion);
        CurrentVersion = currVersion;
    }

    public string GetVersion() {
        return CurrentVersion;
    }

    public byte[] AddVersion(byte[] text) {
        byte[] versionAsBytes = Encoding.Unicode.GetBytes($"{openingChar}{GetVersion()}{closingChar}");
        return versionAsBytes.Concat(text).ToArray();
    }

    public static int GetRfcDerivedBytesIterations(string version) => version switch {
        "test.1" => 1000,
        "test.2" => 20000,
        "1.00" => 500000,
        _ => throw new NotImplementedException($"The given version hasn't been implemented yet: {version}")
    };

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

