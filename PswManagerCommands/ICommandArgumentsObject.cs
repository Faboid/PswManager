﻿using PswManagerCommands.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands {
    public interface ICommandArgumentsObject : IParseable { //will soon inherit a "IValidable" interface or something like that
    }
}
