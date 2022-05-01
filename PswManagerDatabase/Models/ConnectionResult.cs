using System;

namespace PswManagerDatabase.Models {
    public class ConnectionResult {

        public ConnectionResult(bool success) {
            Success = success;
        }

        public ConnectionResult(bool success, string errorMessage) {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; init; }
        public string ErrorMessage { get; init; }

        public ConnectionResult<TValue> ToConnectionResult<TValue>() {
            if(Success) {
                throw new InvalidCastException("This class can be converted to a child version only when it represents a failed operation.");
            }

            return new(Success, ErrorMessage);
        }

    }

    public class ConnectionResult<TValue> : ConnectionResult {

        public ConnectionResult(bool success) : base(success) { }

        public ConnectionResult(bool success, string errorMessage) : base(success, errorMessage) { }
        public ConnectionResult(bool success, TValue value) : base(success) {
            Value = value;
        }

        public TValue Value { get; init; }

    }

    public class AccountResult : ConnectionResult<AccountModel> {

        public AccountResult(bool success) : base(success) { }
        public AccountResult(bool success, string errorMessage) : base(success, errorMessage) { }
        public AccountResult(bool success, AccountModel value) : base(success, value) { }

    }

}
