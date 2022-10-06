﻿using PswManager.Commands.Validation.Attributes;
using PswManager.Commands.Validation.Models;
using PswManager.ConsoleUI.Commands.Validation.Attributes;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Interfaces;

namespace PswManager.ConsoleUI.Commands.Validation.ValidationTypes;

/// <summary>
/// Verifies if the account's existence is the expected.
/// </summary>
public class VerifyAccountExistenceRule : ValidationRule {

    private readonly IDataHelper dataHelper;

    public VerifyAccountExistenceRule(IDataHelper dataHelper) : base(typeof(VerifyAccountExistenceAttribute), typeof(string)) {
        this.dataHelper = dataHelper;
    }

    protected override bool InnerLogic(RuleAttribute attribute, object value) {

        var expected = (attribute as VerifyAccountExistenceAttribute).ShouldExist ? AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
        return dataHelper.AccountExist((string)value) == expected;
    }
}
