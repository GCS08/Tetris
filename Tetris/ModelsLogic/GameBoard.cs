using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class GameBoard : GameBoardModel
    {
        public GameBoard()
        {
            int rows = ConstData.GameGridRowCount;
            int cols = ConstData.GameGridColumnCount;
            Board = new Cube[rows, cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    Board[r, c] = new (ConstData.GameGridColumnWidth,
                        ConstData.GameGridRowHeight, Colors.Transparent);
        }

        // Converts the logical grid into a visual representation.
        public BoxView[,] ToBoxViews()
        {
            int rows = Board!.GetLength(0);
            int cols = Board!.GetLength(1);
            BoxView[,] visuals = new BoxView[rows, cols];

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    Cube cube = Board[r, c];
                    visuals[r, c] = new BoxView
                    {
                        Color = cube.Color,
                        WidthRequest = cube.Width,
                        HeightRequest = cube.Height
                    };
                }
            return visuals;
        }
    }

}
