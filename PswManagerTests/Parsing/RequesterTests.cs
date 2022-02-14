using PswManagerLibrary.InputBuilder;
using PswManagerLibrary.InputBuilder.Attributes;
using PswManagerLibrary.UIConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Parsing {
    public class RequesterTests {

        [Fact]
        public void Success() {

            //arrange
            string name = "exit";
            string invalidInputNull = null;
            string invalidInputEmpty = "  ";
            string password = "rugtyrh&&ieow";
            List<string> answers = new() { name, invalidInputEmpty, invalidInputNull, invalidInputEmpty, password };
            List<bool> yesOrNoAns = new() { false, true };
            FakeUserInput fakeUserInput = new(answers, yesOrNoAns);
            Requester requester = new(typeof(InputObject), fakeUserInput);

            //act
            var success = requester.Build(out var obj);

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
            var success = requester.Build(out var obj);

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
            var success = requester.Build(out var obj);

            //assert
            Assert.False(success);
            Assert.True(obj is default(InputObject));

        }


    }

    internal class InputObject {

        [Request("Input the account's name.")]
        public string Name { get; set; }

        [Request("Input the password.")]
        public string Password { get; set; }

        [Request("Input the email.", true)]
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
            var ans = answers.FirstOrDefault();
            answers.Remove(ans);
            return ans;
        }

        public string RequestAnswer() {
            var ans = answers.FirstOrDefault();
            answers.Remove(ans);
            return ans;
        }

        public void SendMessage(string message) {
            
        }

        public bool YesOrNo(string question) {
            var ans = yesOrNoAnswers.First();
            yesOrNoAnswers.Remove(ans);
            return ans;
        }
    }
}
