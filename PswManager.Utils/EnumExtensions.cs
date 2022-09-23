using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManager.Utils; 
public static class EnumExtensions {

    public static bool IsEqual<TEnum>(this TEnum a, TEnum b) where TEnum : Enum {
        return EqualityComparer<TEnum>.Default.Equals(a, b);
    }

}
