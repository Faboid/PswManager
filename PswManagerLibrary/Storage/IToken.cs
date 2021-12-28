namespace PswManagerLibrary.Storage {
    public interface IToken {

        public bool GetUserConfirmation(out string message);

        void Set(string passToken, string emaToken);

        bool Confront(string passToken, string emaToken);

        (string passToken, string emaToken)? TryGet();

    }
}
