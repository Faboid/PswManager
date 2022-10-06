namespace PswManager.Extensions;
public static class EnumExtensions {

    /// <summary>
    /// Checks the given enums for equality.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool IsEqual<TEnum>(this TEnum a, TEnum b) where TEnum : Enum {
        return EqualityComparer<TEnum>.Default.Equals(a, b);
    }

}
