namespace PswManager.Database.DataAccess.ErrorCodes;

public enum CreatorErrorCode {
    Undefined,
    InvalidName,
    MissingPassword,
    MissingEmail,
    AccountExistsAlready,
}

public enum ReaderErrorCode {
    Undefined,
    InvalidName,
    UsedElsewhere,
    DoesNotExist,
}

//to implement
public enum AccountExistsErrorCode {
    Undefined,
}

public enum DeleterErrorCode {
    Undefined,
}

public enum EditorErrorCode {
    Undefined,
}
