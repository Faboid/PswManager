using PswManager.Commands.Validation.Attributes;
using System;

namespace PswManager.UI.Console.Commands.Validation.Attributes;

/// <summary>
/// Metadata on whether the account is expected to exist.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class VerifyAccountExistenceAttribute : RuleAttribute {

    public bool ShouldExist;

    public VerifyAccountExistenceAttribute(bool shouldExist, string errorMessage) : base(errorMessage) {
        ShouldExist = shouldExist;
    }

}
