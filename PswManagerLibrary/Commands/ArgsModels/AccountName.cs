using PswManagerLibrary.InputBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.ArgsModels {
    public class AccountName {

        [Request("Name", "Insert the name of the account you wish to delete.")]
        public string Name { get; set; }

    }
}
