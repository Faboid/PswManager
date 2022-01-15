﻿using PswManagerDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.Interfaces {
    public interface IDataEditor : IDataHelper {

        ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel);

    }
}