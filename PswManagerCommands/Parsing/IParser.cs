using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Parsing {
    public interface IParser {

        void Register(string key, VariableReference<string> reference);

    }
}
