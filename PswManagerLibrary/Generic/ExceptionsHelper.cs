using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Generic {
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
