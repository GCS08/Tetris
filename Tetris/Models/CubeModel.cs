using CommunityToolkit.Mvvm.ComponentModel;

namespace Tetris.Models;

public partial class CubeModel(double width, double height,
    Color? color) : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    public double Width { get; } = width;
    public double Height { get; } = height;
    private Color? color = color;
    public Color? Color
    {
        get => color;
        set => SetProperty(ref color, value);
    }
}
