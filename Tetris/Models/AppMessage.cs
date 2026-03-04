using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Tetris.Models
{
    /// <summary>
    /// Represents an application message that encapsulates a value change of type T.
    /// </summary>
    /// <typeparam name="T">The type of the value being communicated.</typeparam>
    /// <param name="msg">The value to be sent with the message.</param>
    public class AppMessage<T>(T msg) : ValueChangedMessage<T>(msg)
    {
    }
}
