using PswManagerCommands.Validation.Attributes;
using PswManagerCommands.Validation.Models;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Commands.Validation.Attributes;

namespace PswManagerLibrary.Commands.Validation.ValidationLogic {
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
