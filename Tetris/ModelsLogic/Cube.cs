using Tetris.Models;

namespace Tetris.ModelsLogic
{
    /// <summary>
    /// Represents a 2D cube with specified dimensions and color.
    /// </summary>
    /// <param name="color">The color of the cube.</param>
    public class Cube (Color color) : CubeModel(color)
    { }
}
