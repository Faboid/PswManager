using PswManager.Database.Models;
using System;

namespace PswManager.Database.DataAccess.ErrorCodes;

internal static class CodeConverter {

    /// <summary>
    /// Use only if <paramref name="valid"/>(<see cref="AccountValid"/>) is faulty.
    /// <br/>Converts <paramref name="valid"/> to <see cref="CreatorErrorCode"/>.
    /// <br/>
    /// <br/>
    /// Note: attempting the conversion when <paramref name="valid"/> is <see cref="AccountValid.Valid"/> will throw an <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="valid"></param>
    /// <param name="errorCode"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static CreatorErrorCode ToCreatorErrorCode(this AccountValid valid) => valid switch {
        AccountValid.Valid => throw new ArgumentException("This conversion is only supported if the given AccountValid is NOT Valid.", nameof(valid)),
        AccountValid.MissingName => CreatorErrorCode.InvalidName,
        AccountValid.MissingPassword => CreatorErrorCode.MissingPassword,
        AccountValid.MissingEmail => CreatorErrorCode.MissingEmail,
        _ => CreatorErrorCode.Undefined,
    };

}
