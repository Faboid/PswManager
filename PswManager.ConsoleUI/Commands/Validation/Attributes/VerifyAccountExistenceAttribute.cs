using PswManager.Commands.Validation.Attributes;
using System;

namespace PswManager.ConsoleUI.Commands.Validation.Attributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class VerifyAccountExistenceAttribute : RuleAttribute {

        public bool ShouldExist;

        public VerifyAccountExistenceAttribute(bool shouldExist, string errorMessage) : base(errorMessage) {
            ShouldExist = shouldExist;
        }

    }
}
