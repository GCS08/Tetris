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
        private Random rnd = new();

        public ShapeModel(int Id, List<bool[,]>? RotationStates, int TopLeftX, int TopLeftY, Color color)
        {
            this.Id = Id;
            this.RotationStates = RotationStates;
            this.TopLeftX = TopLeftX;
            this.TopLeftY = TopLeftY;
            this.Color = color;
        }
        public ShapeModel()
        {
            this.Id = rnd.Next(ConstData.ShapesCount);
        }
    }
}
