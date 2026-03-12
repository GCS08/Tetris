namespace Tetris.Models;

/// <summary>
/// Represents a cube with specified width, height, and optional color, supporting property change notifications.
/// </summary>
/// <param name="color">The color of the cube, or null for default.</param>
public class Cube(Color? color) : 
    CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    #region Fields
    private Color? color = color;
    #endregion

    #region Properties
    public Color? Color
    {
        get => color;
        set => SetProperty(ref color, value);
    }
    #endregion
}
