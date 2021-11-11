using PswManagerLibrary.UIConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerConsole {
    public class UserInput : IUserInput {

        public string RequestAnswer() {
            return Console.ReadLine();
        }

        public string RequestAnswer(string message) {
            SendMessage(message);
            return RequestAnswer();
        }

        public void SendMessage(string message) {
            Console.WriteLine(message);
        }

        public bool YesOrNo(string question) {

            Console.WriteLine(question);
            Console.WriteLine("Y/N");

            //this loop is broken only by a return given by the user's answer.
            while(true) {

                string answer = Console.ReadLine();

                if(new[] { "y", "yes" }.Any(x => x == answer.ToLowerInvariant())) {
                    return true;
                } else if(new[] { "n", "no" }.Any(x => x == answer.ToLowerInvariant())) {
                    return false;
                } else {
                    Console.WriteLine("Invalid response. Please write either \"yes\" or \"no\"");
                }

            }
        }
    }
}
