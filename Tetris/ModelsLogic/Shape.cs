using Tetris.Models;
using Tetris.Converters;

namespace Tetris.ModelsLogic
{
    public class Shape : ShapeModel
    {
        public Shape(int inGameId)
        {
            this.Id = rnd.Next(ConstData.ShapesCount);
            this.InGameId = inGameId;
            this.RotationStates = ConstData.ShapeRotationState[this.Id];
            this.Color = ConstData.colors[rnd.Next(ConstData.colors.Length)];
            this.TopLeftX = (ConstData.GameGridColumnCount - Cells.GetLength(1)) / 2;
            this.TopLeftY = 0;
        }
        public Shape(int id, int inGameId, string color)
        {
            this.Id = id;
            this.InGameId = inGameId;
            this.RotationStates = ConstData.ShapeRotationState[this.Id];
            this.Color = StringAndColorConverter.ColorNameToColor(color);
            this.TopLeftX = (ConstData.GameGridColumnCount - Cells.GetLength(1)) / 2;
            this.TopLeftY = 0;
        }
        public override Shape Duplicate(Shape shape)
        {
            if (shape.Color == null) return null!;
            return new Shape(shape.Id, shape.InGameId, StringAndColorConverter.ColorToColorName(shape.Color));
        }
    }
}
