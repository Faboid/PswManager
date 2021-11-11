using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.UIConnection {
    public interface IUserInput {

        bool YesOrNo(string question);

        void SendMessage(string message);

        string RequestAnswer(string message);

        string RequestAnswer();
    }
}
