using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase.Models {
    public class ConnectionResult {

        public ConnectionResult(bool success, string errorMessage = null) {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; init; }
        public string ErrorMessage { get; init; }

    }

    public class ConnectionResult<TValue> : ConnectionResult where TValue : class {
        public ConnectionResult(bool success, string errorMessage = null) : base(success, errorMessage) {
        }

        public ConnectionResult(bool success, string errorMessage = null, TValue value = null) : base(success, errorMessage) {
            Value = value;
        }

        public ConnectionResult(bool success, TValue value = null) : base(success) {
            Value = value;
        }


        public TValue Value { get; init; }

    }

}
