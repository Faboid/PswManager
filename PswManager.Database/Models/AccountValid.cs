namespace PswManager.Database.Models;

internal enum AccountValid {
    Undefined,
    Valid,
    MissingName,
    MissingPassword,
    MissingEmail,
    IsNull
}

