using Tetris.Models;
using Tetris.Converters;

namespace Tetris.ModelsLogic
{
    public class Shape : ShapeModel
    {
        public Shape()
        {
            this.Id = rnd.Next(ConstData.ShapesCount);
            this.RotationStates = ConstData.ShapeRotationState[this.Id];
            this.Color = ConstData.colors[rnd.Next(ConstData.colors.Length)];
            this.TopLeftX = (ConstData.GameGridColumnCount - Cells.GetLength(1)) / 2;
            this.TopLeftY = 0;
        }
        public Shape(int id, string color)
        {
            this.Id = id;
            this.RotationStates = ConstData.ShapeRotationState[this.Id];
            this.Color = StringAndColorConverter.ColorNameToColor(color);
            this.TopLeftX = (ConstData.GameGridColumnCount - Cells.GetLength(1)) / 2;
            this.TopLeftY = 0;
        }
        public Shape(int Id, List<bool[,]>? RotationStates, int TopLeftX, int TopLeftY, Color color)
        {
            this.Id = Id;
            this.RotationStates = RotationStates;
            this.TopLeftX = TopLeftX;
            this.TopLeftY = TopLeftY;
            this.Color = color;
        }
    }
}
