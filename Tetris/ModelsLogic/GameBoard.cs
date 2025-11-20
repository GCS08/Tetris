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
    }
}
