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

//to implement
public enum AccountExistsErrorCode {
    Undefined,
}

public enum DeleterErrorCode {
    Undefined,
    InvalidName,
    UsedElsewhere,
    DoesNotExist,
}

public enum EditorErrorCode {
    Undefined,
}
