using Tetris.Models;

namespace Tetris.ModelsLogic
{
    /// <summary>
    /// Represents a 2D cube with specified dimensions and color.
    /// </summary>
    /// <param name="width">The width of the cube.</param>
    /// <param name="height">The height of the cube.</param>
    /// <param name="color">The color of the cube.</param>
    public class Cube (double width, double height,
        Color color) : CubeModel(width, height, color)
    { }
}
