﻿using PswManager.Commands.Validation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManager.Core.Commands.Validation.Attributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class VerifyAccountExistenceAttribute : RuleAttribute {

        public bool ShouldExist;

        public VerifyAccountExistenceAttribute(bool shouldExist, string errorMessage) : base(errorMessage) { 
            ShouldExist = shouldExist;
        }

    }
}