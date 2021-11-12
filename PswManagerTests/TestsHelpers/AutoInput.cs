using PswManagerLibrary.UIConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerTests.TestsHelpers {

    /// <summary>
    /// Gives custom returns. 
    /// </summary>
    public class AutoInput : IUserInput {

        public string ReturnString = "DefaultComputerAnswer";
        public bool YesOrNoReturn = true;

        public void SetDefault() {
            ReturnString = "DefaultComputerAnswer";
            YesOrNoReturn = true;
        }

        public string RequestAnswer(string message) {
            return ReturnString;

        }

        public string RequestAnswer() {
            return ReturnString;

        }

        public void SendMessage(string message) {
            
        }

        public bool YesOrNo(string question) {
            return YesOrNoReturn;
        }
    }
}
