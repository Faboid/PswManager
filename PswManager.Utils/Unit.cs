using System.Runtime.InteropServices;
namespace PswManager.Utils;

/// <summary>
/// Represents void.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 1)]
public sealed class Unit {

    /// <summary>
    /// Gets an empty instance of <see cref="Unit"/>.
    /// </summary>
    public readonly static Unit Get = new();
    private Unit() { }

}