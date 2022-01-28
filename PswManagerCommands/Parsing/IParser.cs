using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Parsing {
    public interface IParser {

        public string Separator { get; }

        bool TryParse<TParseable>(string input, out TParseable parseable) where TParseable : class, IParseable, new();
        void Register(string key, VariableReference<string> reference);

    }
}
