using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Tetris.Models
{
    public class AppMessage<T>(T msg) : ValueChangedMessage<T>(msg)
    {
    }
}
