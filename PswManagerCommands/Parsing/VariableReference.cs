using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Parsing {
    //this class idea has been taken from Timwi's answer in https://stackoverflow.com/questions/24329012/store-reference-to-an-object-in-dictionary
    public class VariableReference<T> {

        public Func<T> Get { get; private set; }
        public Action<T> Set { get; private set; }

        public VariableReference(Func<T> getter, Action<T> setter) {
            Get = getter;
            Set = setter;
        }

    }
}
