using System;

namespace PswManager.Utils {
    public static class ExceptionsHelper {

        public static void IfTrueThrow(this bool condition, Exception exception) {
            if(condition is true) {
                throw exception;
            }
        }

        public static void IfFalseThrow(this bool condition, Exception exception) {
            if(condition is false) {
                throw exception;
            }
        }

    }
}
