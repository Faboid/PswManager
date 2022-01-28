using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Parsing {
    public interface IParseable {

        public void RegisterAll(IParser parser);

    }
}
