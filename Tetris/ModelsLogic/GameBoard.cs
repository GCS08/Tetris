using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class GameBoard : GameBoardModel
    {
        public GameBoard()
        {
            Board = new Cube[ConstData.GameGridRowCount, ConstData.GameGridColumnCount];

            for (int r = 0; r < ConstData.GameGridRowCount; r++)
            {
                for (int c = 0; c < ConstData.GameGridColumnCount; c++)
                {
                    Board[r, c] = new Cube(
                        ConstData.GameGridColumnWidth,
                        ConstData.GameGridRowHeight,
                        Colors.Transparent
                    );
                }
            }
        }
        public void ShowShape(Queue<Shape> queue)
        {
            Shape currentShape = queue.Remove();
            for (int i = 0; i < currentShape.Cells.GetLength(0); i++)
                for (int j = 0; j < currentShape.Cells.GetLength(1); j++)
                    if (currentShape.Cells[i,j])
                        Board![i + currentShape.TopLeftX, j + currentShape.
                            TopLeftY].Color = currentShape.Color;
        }
    }
}
