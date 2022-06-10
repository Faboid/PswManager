using PswManager.Commands.Validation.Attributes;
using PswManager.Commands.Validation.Models;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.ConsoleUI.Commands.Validation.Attributes;
using PswManager.Database.DataAccess.ErrorCodes;

namespace PswManager.ConsoleUI.Commands.Validation.ValidationTypes {
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
}
