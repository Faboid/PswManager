﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Validation.Attributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAttribute : Attribute {
    }
}