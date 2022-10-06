namespace PswManager.UI.WPF.ConstantValues;
#if DEBUG

/// <summary>
/// This is a debug-only class that provides constant values for an easier debugging. It does not exist in Release.
/// </summary>
public static class Debugging {

    /// <summary>
    /// A constant master key to use for debugging. Does not exist in Release.
    /// </summary>
    public const string MasterKey = "ThisisAValidPassword, Only to be used for debugging";

}
#endif 