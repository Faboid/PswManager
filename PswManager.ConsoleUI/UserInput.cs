using PswManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManager.ConsoleUI;
public class UserInput : IUserInput {

    public string RequestAnswer() {
        return Console.ReadLine();
    }

    public string RequestAnswer(string message) {
        Console.WriteLine(message);
        return RequestAnswer();
    }

    public char[] RequestPassword() {
        List<char> output = new();

        char asChar;
        int asNum;
        do {
            asChar = Console.ReadKey().KeyChar;
            asNum = asChar;

            //between space and tilde
            if(asNum >= 32 && asNum <= 126) {
                output.Add(asChar);
            }
            //delete key
            if(asNum == 8 && output.Count > 0) {
                output.RemoveAt(output.Count - 1);
            }

            //clears the line
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.BufferWidth));

            //fills line with asterisks
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string('*', output.Count));

            //while it's not enter
        } while(asNum != 13);

        Console.WriteLine();
        return output.ToArray();
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
