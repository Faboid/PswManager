using PswManagerDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.Interfaces {
    public interface IDataDeleter : IDataHelper {
    
        ConnectionResult DeleteAccount(string name);

    }
}
