using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace Tetris.Models;

/// <summary>
/// Provides a base class that implements INotifyPropertyChanged to support property changes.
/// </summary>
public partial class ObservableObject : INotifyPropertyChanged
{
    #region Events
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion

    #region Public Methods
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
