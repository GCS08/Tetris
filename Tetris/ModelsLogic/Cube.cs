using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class Cube : CubeModel
    {
        public Cube(double width, double height, Color color)
        {
            Width = width;
            Height = height;
            Color = color;
        }
    }
}
