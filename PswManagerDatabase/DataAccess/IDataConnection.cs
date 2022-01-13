using PswManagerDatabase.Config;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess {
    public interface IDataConnection : IPathsEditor, IDataCreator, IDataReader, IDataEditor, IDataDeleter {


    }
}
