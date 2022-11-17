using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PswManager.Encryption.Cryptography;

/// <summary>
/// Acts as a key for cryptographic operations.
/// </summary>
public class Key : IDisposable {

    /// <summary>
    /// Initializes a <see cref="Key"/> with the given <paramref name="password"/>.
    /// <remarks>
    /// All the <see cref="byte"/>(s) in <paramref name="password"/> will be set to <see cref="0"/>.
    /// </remarks>
    /// </summary>
    /// <param name="password"></param>
    public Key(byte[] password) : this(Encoding.Unicode.GetChars(password)) {
        for(int i = 0; i < password.Length; i++) {
            password[i] = 0;
        }
    }

    /// <summary>
    /// Initializes a <see cref="Key"/> with the given <paramref name="password"/>.
    /// <remarks>
    /// All the <see cref="char"/>(s) in <paramref name="password"/> will be set to <see langword="default"/>.
    /// </remarks>
    /// </summary>
    /// <param name="password"></param>
    public Key(char[] password) {
        key = new char[password.Length];
        gcHandle = GCHandle.Alloc(key, GCHandleType.Pinned);
        password.CopyTo(key, 0);
        Clear(password);
    }

    private readonly char[] key;
    private GCHandle gcHandle;

    public char[] Get() {
        return key;
    }

    private static void Clear(char[] arr) {
        for(int i = 0; i < arr.Length; i++) {
            arr[i] = default;
        }
    }

    public void Dispose() {
        Clear(key);
        gcHandle.Free();
        GC.SuppressFinalize(this);
    }

    public static bool operator ==(Key? left, Key? right) {

        //true if both are null, false if only one is null
        if(left is null || right is null) {
            return left is null && right is null;
        }

        var l = left.Get();
        var r = right.Get();

        if(l.Length != r.Length) {
            return false;
        }

        return l.SequenceEqual(r);
    }

    public static bool operator !=(Key left, Key right) => !(left == right);

    public override bool Equals(object? obj) {
        return obj is Key key && this == key;    
    }

    public override int GetHashCode() {
        return key.GetHashCode();
    }
}
