using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class ShapeModel
    {
        public int Id { get; set; }
        public int InGameId { get; set; }
        public List<bool[,]>? RotationStates { get; set; }
        public int RotationIndex { get; set; } = 0;
        public bool[,] Cells => RotationStates![RotationIndex];
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }
        public Color? Color { get; set; }
        protected readonly Random rnd = new();
        public abstract Shape Duplicate(Shape shape);
    }
}
