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

    }

    public class ConnectionResult<TValue> : ConnectionResult {

        public ConnectionResult(bool success) : base(success) { }

        public ConnectionResult(bool success, string errorMessage) : base(success, errorMessage) { }
        public ConnectionResult(bool success, TValue value) : base(success) {
            Value = value;
        }

        public TValue Value { get; init; }

    }

}
