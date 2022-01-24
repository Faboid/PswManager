using PswManagerLibrary.UIConnection;
using System;

namespace PswManagerTests.TestsHelpers {

    /// <summary>
    /// Gives custom returns. 
    /// </summary>
    [Obsolete("This will be substituted by mocking through Moq.")]
    public class AutoInput : IUserInput {
        //todo - delete this class
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
