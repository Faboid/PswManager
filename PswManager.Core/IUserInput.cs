namespace PswManager.Core;
public interface IUserInput {

    bool YesOrNo(string question);

    void SendMessage(string message);

    string RequestAnswer(string message);

    string RequestAnswer();

    char[] RequestPassword();
}
