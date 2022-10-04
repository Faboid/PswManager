using System;

namespace PswManager.UI.WPF.Services;
public interface INotificationService {

    public event Action<string>? NewMessage;

    void Send(string message);
    void Send(string message, string title);

}
