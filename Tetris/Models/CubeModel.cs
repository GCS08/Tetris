namespace Tetris.Models;

/// <summary>
/// Represents a cube with specified width, height, and optional color, supporting property change notifications.
/// </summary>
/// <param name="width">The width of the cube.</param>
/// <param name="height">The height of the cube.</param>
/// <param name="color">The color of the cube, or null for default.</param>
public class CubeModel(double width, double height, Color? color) : 
    CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    #region Fields
    private Color? color = color;
    #endregion

    #region Properties
    public double Width { get; } = width;
    public double Height { get; } = height;
    public Color? Color
    {
        get => color;
        set => SetProperty(ref color, value);
    }
    #endregion
}
