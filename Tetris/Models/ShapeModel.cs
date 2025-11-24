namespace Tetris.Models
{
    public class ShapeModel
    {
        public int Id { get; set; }
        public List<bool[,]>? RotationStates { get; set; }
        public int RotationIndex { get; set; } = 0;
        public bool[,] Cells => RotationStates![RotationIndex];
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }
        public Color? Color { get; set; }
        protected readonly Random rnd = new();
    }
}
