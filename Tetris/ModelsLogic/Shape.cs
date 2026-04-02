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
        /// <param name="inGameId">
        /// The unique identifier for this shape instance within the current game.
        /// </param>
        public Shape(int inGameId)
        {
            this.Id = rnd.Next(ConstData.ShapesCount);
            this.InGameId = inGameId;
            this.RotationStates = ConstData.ShapeRotationState[this.Id];
            this.Color = ConstData.colors[rnd.Next(ConstData.colors.Length)];
            this.TopLeftX = (ConstData.GameGridColumnCount - Cells.GetLength(1)) / 2;
            this.TopLeftY = 0;
        }

        /// <summary>
        /// Initializes a new <see cref="Shape"/> with a specified type, color, and in-game ID.
        /// </summary>
        /// <param name="id">
        /// The shape type identifier (index into <see cref="ConstData.ShapeRotationState"/>).
        /// </param>
        /// <param name="inGameId">
        /// The unique identifier for this shape instance within the current game.
        /// </param>
        /// <param name="color">
        /// The color of the shape, as a string name (converted to a <see cref="Color"/>).
        /// </param>
        public Shape(int id, int inGameId, string color)
        {
            this.Id = id;
            this.InGameId = inGameId;
            this.RotationStates = ConstData.ShapeRotationState[this.Id];
            this.Color = StringAndColorConverter.ColorNameToColor(color);
            this.TopLeftX = (ConstData.GameGridColumnCount - Cells.GetLength(1)) / 2;
            this.TopLeftY = 0;
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
                    InGameId,
                    StringAndColorConverter.ColorToColorName(Color)
                );
            return result!;
        }

        #endregion
    }
}