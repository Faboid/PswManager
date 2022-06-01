using PswManager.Commands.Validation.Attributes;
using PswManager.Commands.Validation.Models;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.ConsoleUI.Commands.Validation.Attributes;

namespace PswManager.ConsoleUI.Commands.Validation.ValidationTypes {
    public class VerifyAccountExistenceRule : ValidationRule {

        private readonly IDataHelper dataHelper;

        public VerifyAccountExistenceRule(IDataHelper dataHelper) : base(typeof(VerifyAccountExistenceAttribute), typeof(string)) {
            this.dataHelper = dataHelper;
        }

        protected override bool InnerLogic(RuleAttribute attribute, object value) {

            return dataHelper.AccountExist((string)value) == (attribute as VerifyAccountExistenceAttribute).ShouldExist;
        }
    }
}
