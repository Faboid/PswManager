﻿using PswManager.UI.Console.Attributes;

namespace PswManager.UI.Console.Tests;
public class RequesterTests {

    [Fact]
    public void Success() {

        //arrange
        string name = "exit";
        string invalidInputNull = null;
        string invalidInputEmpty = "  ";
        string password = "rugtyrh&&ieow";
        List<string> answers = new() { name, invalidInputEmpty, invalidInputNull, invalidInputEmpty, password, default };
        List<bool> yesOrNoAns = new() { false, true };
        FakeUserInput fakeUserInput = new(answers, yesOrNoAns);
        Requester requester = new(typeof(InputObject), fakeUserInput);

        //act
        var (success, obj) = requester.Build();

        //assert
        Assert.True(success);
        Assert.True(obj is InputObject);
        var output = (InputObject)obj;
        Assert.Equal(name, output.Name);
        Assert.Equal(password, output.Password);
        Assert.Equal(default, output.Email);

    }

    [Fact]
    public void Failure_UserClaimedWrongValues() {

        //arrange
        string name = "Yehallo";
        string password = "rugtyrh&&ieow";
        string email = "email@here.com";
        List<string> answers = new() { name, password, email };
        List<bool> yesOrNoAns = new() { false };
        FakeUserInput fakeUserInput = new(answers, yesOrNoAns);
        Requester requester = new(typeof(InputObject), fakeUserInput);

        //act
        var (success, obj) = requester.Build();

        //assert
        Assert.False(success);
        Assert.True(obj is InputObject);

    }

    [Fact]
    public void Failure_UserExitedEarly() {

        //arrange
        string name = "Yehallo";
        string invalidInput = "";
        string exit = "exit";
        List<string> answers = new() { name, invalidInput, exit };
        List<bool> yesOrNoAns = new() { true };
        FakeUserInput fakeUserInput = new(answers, yesOrNoAns);
        Requester requester = new(typeof(InputObject), fakeUserInput);

        //act
        var (success, obj) = requester.Build();

        //assert
        Assert.False(success);
        Assert.True(obj is default(InputObject));

    }

}

internal class InputObject {

    [Request("Name", "Input the account's name.")]
    public string Name { get; set; }

    [Request("Password", "Input the password.")]
    public string Password { get; set; }

    [Request("Email", "Input the email.", true)]
    public string Email { get; set; }
}

internal class FakeUserInput : IUserInput {

    public FakeUserInput(List<string> answers, List<bool> yesOrNoAnswers) {
        this.answers = answers;
        this.yesOrNoAnswers = yesOrNoAnswers;
    }

    private readonly List<string> answers;
    private readonly List<bool> yesOrNoAnswers;

    public string RequestAnswer(string message) {
        var ans = answers.First();
        answers.Remove(ans);
        return ans;
    }

    public string RequestAnswer() {
        var ans = answers.First();
        answers.Remove(ans);
        return ans;
    }

    public void SendMessage(string message) {

    }

    public char[] RequestPassword() {
        throw new NotImplementedException();
    }

    public bool YesOrNo(string question) {
        var ans = yesOrNoAnswers.First();
        yesOrNoAnswers.Remove(ans);
        return ans;
    }
}
