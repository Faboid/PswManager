using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Parsing {
    public interface IParser {

        public string Separator { get; }
        public char Equal { get; }

        IParserReady Setup<TParseable>() where TParseable : ICommandArguments, new();

    }

    public interface IParserReady {

        ParsingResult Parse(string input);

    }
}
