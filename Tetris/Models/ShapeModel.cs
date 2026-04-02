using Tetris.ModelsLogic;

namespace Tetris.Models
{
    /// <summary>
    /// Represents an abstract base class for shape models, providing properties for identification, rotation states,
    /// position, and color, as well as a method for duplicating shapes.
    /// </summary>
    public abstract class ShapeModel
    {
        #region Fields
        protected readonly Random rnd = new();
        #endregion

        #region Properties
        public int Id { get; set; }
        public int InGameId { get; set; }
        public List<bool[,]>? RotationStates { get; set; }
        public int RotationIndex { get; set; } = 0;
        public bool[,] Cells
        {
            get
            {
                if (RotationStates == null) return null!;
                return RotationStates[RotationIndex];
            }
        }
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }
        public Color? Color { get; set; }
        #endregion

        #region Public Methods
        public abstract Shape Duplicate();
        #endregion
    }
}
