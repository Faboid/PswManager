﻿using PswManager.Commands.Validation.Attributes;

namespace PswManager.UI.Console.Commands.Validation.Attributes;

/// <summary>
/// Specifies that there shouldn't be duplicate values.
/// </summary>
public class NoDuplicateValuesAttribute : RuleAttribute {

    public NoDuplicateValuesAttribute(string errorMessage) : base(errorMessage) {

    }

}
