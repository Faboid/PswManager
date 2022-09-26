using System;
using System.Collections.Generic;

namespace PswManager.Utils;
public static class EnumExtensions {

    public static bool IsEqual<TEnum>(this TEnum a, TEnum b) where TEnum : Enum {
        return EqualityComparer<TEnum>.Default.Equals(a, b);
    }

}
