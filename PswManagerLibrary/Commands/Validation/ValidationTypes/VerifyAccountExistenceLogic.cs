using PswManagerCommands.Validation.Models;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Commands.Validation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.Validation.ValidationLogic {
    public class VerifyAccountExistenceLogic : ValidationLogic<VerifyAccountExistenceAttribute, string> {

        private readonly IDataHelper dataHelper;

        public VerifyAccountExistenceLogic(IDataHelper dataHelper) {
            this.dataHelper = dataHelper;
        }

        public override bool Validate(VerifyAccountExistenceAttribute attribute, string value, out string errorMessage) {
            errorMessage = string.Empty;
            bool valid = dataHelper.AccountExist(value) == attribute.ShouldExist;

            if(!valid) {
                errorMessage = attribute.ShouldExist switch {
                    true => "The given account doesn't exist.",
                    false => "The account exists already."
                };
            }

            return valid;
        }
    }
}
