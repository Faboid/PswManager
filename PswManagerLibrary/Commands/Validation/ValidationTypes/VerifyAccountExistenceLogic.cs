using PswManagerCommands.Validation.Models;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Commands.Validation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.Validation.ValidationLogic {
    public class VerifyAccountExistenceLogic : PswManagerCommands.Validation.Models.ValidationLogic {

        private readonly IDataHelper dataHelper;

        public VerifyAccountExistenceLogic(IDataHelper dataHelper) : base(typeof(VerifyAccountExistenceAttribute), typeof(string)) {
            this.dataHelper = dataHelper;
        }

        protected override bool InnerLogic(Attribute attribute, object value) {

            return dataHelper.AccountExist((string)value) == (attribute as VerifyAccountExistenceAttribute).ShouldExist;
        }
    }
}
