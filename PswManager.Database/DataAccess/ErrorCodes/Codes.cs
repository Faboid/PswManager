namespace PswManager.Database.DataAccess.ErrorCodes;

public enum CreatorErrorCode {
    Undefined,
    InvalidName,
    MissingPassword,
    MissingEmail,
    AccountExistsAlready,
    UsedElsewhere,
}

public enum ReaderErrorCode {
    Undefined,
    InvalidName,
    UsedElsewhere,
    DoesNotExist,
}

public enum ReaderAllErrorCode {
    Undefined,
    SomeUsedElsewhere,
}

public enum AccountExistsStatus {
    /// <summary>
    /// An unknown error has occurred.
    /// </summary>
    Undefined,
    /// <summary>
    /// The account has been found and it exists.
    /// </summary>
    Exist,
    /// <summary>
    /// The account hasn't been found.
    /// </summary>
    NotExist,
    /// <summary>
    /// The given name is invalid.
    /// </summary>
    InvalidName,
    /// <summary>
    /// This account is being used elsewhere, so it's not possible to verify it's existence.
    /// </summary>
    UsedElsewhere,
}

public enum DeleterErrorCode {
    Undefined,
    InvalidName,
    UsedElsewhere,
    DoesNotExist,
}

public enum EditorErrorCode {
    /// <summary>
    /// The error is undefined. This could either created with <see langword="default"/> or because an unknown error happened.
    /// </summary>
    Undefined,
    /// <summary>
    /// The given name is null, empty, or whitespace.
    /// </summary>
    InvalidName,
    /// <summary>
    /// The account is being used elsewhere.
    /// </summary>
    UsedElsewhere,
    /// <summary>
    /// The given name does not exist.
    /// </summary>
    DoesNotExist,
    /// <summary>
    /// The new name is being used elsewhere. Might be because this edit request has been launched multiple times, or because that new name is occupied.
    /// </summary>
    NewNameUsedElsewhere,
    /// <summary>
    /// The new name exists already, so the account cannot be use it.
    /// </summary>
    NewNameExistsAlready,
}
