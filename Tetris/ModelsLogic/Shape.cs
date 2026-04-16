using Tetris.Models;
using Tetris.Converters;

namespace Tetris.ModelsLogic
{
    /// <summary>
    /// Represents a Tetris shape with a specific type, color, rotation states, 
    /// and position on the game grid.
    /// </summary>
    public class Shape : ShapeModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Shape"/> with a random type and color.
        /// </summary>
        public Shape()
        {
            this.Id = rnd.Next(ConstData.ShapesCount);
            this.RotationStates = ConstData.ShapeRotationState[this.Id];
            this.Color = ConstData.colors[rnd.Next(ConstData.colors.Length)];
            this.TopLeftX = (ConstData.GameGridColumnCount - Cells.GetLength(1)) / 2;
            this.TopLeftY = 0;
        }

        /// <summary>
        /// Initializes a new <see cref="Shape"/> with a specified type, color.
        /// </summary>
        /// <param name="id">
        /// The shape type identifier (index into <see cref="ConstData.ShapeRotationState"/>).
        /// </param>
        /// <param name="color">
        /// The color of the shape, as a string name (converted to a <see cref="Color"/>).
        /// </param>
        public Shape(int id, string color)
        {
            this.Id = id;
            this.RotationStates = ConstData.ShapeRotationState[this.Id];
            this.Color = StringAndColorConverter.ColorNameToColor(color);
            this.TopLeftX = (ConstData.GameGridColumnCount - Cells.GetLength(1)) / 2;
            this.TopLeftY = 0;
        }

        /// <summary>
        /// Initializes a new <see cref="Shape"/> with a specified type, color, rotation index, and position.
        /// </summary>
        /// <param name="id">
        /// The shape type identifier (index into <see cref="ConstData.ShapeRotationState"/>).
        /// </param>
        /// <param name="color">
        /// The color of the shape, as a string name (converted to a <see cref="Color"/>).
        /// </param>
        /// <param name="rotationIndex">
        /// The current rotation index of the shape.
        /// </param>
        /// <param name="topLeftX">
        /// The X-coordinate of the shape's top-left corner on the game grid.
        /// </param>
        /// <param name="topLeftY">
        /// The Y-coordinate of the shape's top-left corner on the game grid.
        /// </param>
        public Shape(int id, string color, int rotationIndex, int topLeftX, int topLeftY)
        {
            this.Id = id;
            this.RotationStates = ConstData.ShapeRotationState[this.Id];
            this.RotationIndex = rotationIndex;
            this.Color = StringAndColorConverter.ColorNameToColor(color);
            this.TopLeftX = topLeftX;
            this.TopLeftY = topLeftY;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a duplicate of the given shape, preserving its type, in-game ID, and color.
        /// </summary>
        /// <returns>
        /// A new <see cref="Shape"/> instance with the same Id, InGameId, and <see cref="Color"/>.
        /// Returns <c>null</c> if the original shape's color is <c>null</c>.
        /// </returns>
        public override Shape Duplicate()
        {
            Shape? result = null;
            if (Color != null)
                result = new Shape(
                    Id,
                    StringAndColorConverter.ColorToColorName(Color)
                );
            return result!;
        }

        #endregion
    }
}