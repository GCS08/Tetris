using System.Timers;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class GameBoard : GameBoardModel
    {
        public GameBoard(Shape currentShape)
        {
            Board = new Cube[ConstData.GameGridRowCount, ConstData.GameGridColumnCount];
            this.CurrentShape = currentShape;

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
        public void StartGame()
        {
            ShowShape();
            FallTimer.Elapsed += MoveDownShape;
            FallTimer.Start();
        }
        public override void ShowShape()
        {
            for (int i = 0; i < CurrentShape!.Cells.GetLength(0); i++)
                for (int j = 0; j < CurrentShape!.Cells.GetLength(1); j++)
                    if (CurrentShape.Cells[i, j])
                        Board![i + CurrentShape.TopLeftY, j + 
                            CurrentShape.TopLeftX].Color = CurrentShape.Color;   
        }
        private void MoveDownShape(object? sender, ElapsedEventArgs e)
        {
            MoveDownShape();
        }
        private void EraseShape()
        {
            for (int i = 0; i < CurrentShape!.Cells.GetLength(0); i++)
                for (int j = 0; j < CurrentShape!.Cells.GetLength(1); j++)
                    if (CurrentShape.Cells[i, j])
                        Board![i + CurrentShape.TopLeftY, j +
                            CurrentShape.TopLeftX].Color = Colors.Transparent;
        }
        public override void MoveRightShape()
        {
            if (CurrentShape!.TopLeftX + CurrentShape.Cells.GetLength(1)
                < ConstData.GameGridColumnCount)
            {
                EraseShape();
                CurrentShape!.TopLeftX++;
                ShowShape();
            }
        }
        public override void MoveLeftShape()
        {
            if (CurrentShape!.TopLeftX > 0)
            {
                EraseShape();
                CurrentShape.TopLeftX--;
                ShowShape();
            }
        }
        public override void MoveDownShape()
        {
            int shapeBottom = CurrentShape!.TopLeftY + CurrentShape.Cells.GetLength(0);
            bool belowIsEmpty = true;

            if (shapeBottom >= ConstData.GameGridRowCount)
            {
                ContinueToNextShape();
                return;
            }

            for (int x = 0; x < CurrentShape.Cells.GetLength(1); x++)
                if (Board![shapeBottom, CurrentShape.TopLeftX + x]
                    .Color != Colors.Transparent)
                    belowIsEmpty = false;

            if (belowIsEmpty)
            {
                EraseShape();
                CurrentShape.TopLeftY++;  // this is DOWN
                ShowShape();
            }
            else
                ContinueToNextShape();
        }
        private void ContinueToNextShape()
        {
            throw new NotImplementedException();
        }
        public override void RotateShape()
        {
            if (CurrentShape == null) return;

            int nextIndex = (CurrentShape.RotationIndex + 1) % CurrentShape.RotationStates!.Count;
            bool[,] nextCells = CurrentShape.RotationStates[nextIndex];

            EraseShape();

            // Try rotation at the original position first, then small shifts
            if (TryPlaceRotation(nextCells, CurrentShape.TopLeftX, CurrentShape.TopLeftY, out int newX, out int newY))
            {
                CurrentShape.TopLeftX = newX;
                CurrentShape.TopLeftY = newY;
                CurrentShape.RotationIndex = nextIndex;
            }

            ShowShape();
        }
        private bool TryPlaceRotation(bool[,] cells, int x, int y, out int newX, out int newY)
        {
            newX = x;
            newY = y;

            int h = cells.GetLength(0);
            int w = cells.GetLength(1);

            // Prioritized shifts (center first, then small wall kicks)
            int[] shiftsX = { 0, -1, 1, -2, 2 };
            int[] shiftsY = { 0, -1, 1 };

            foreach (int dx in shiftsX)
            {
                foreach (int dy in shiftsY)
                {
                    bool canPlace = true;

                    for (int i = 0; i < h && canPlace; i++)
                    {
                        for (int j = 0; j < w && canPlace; j++)
                        {
                            if (!cells[i, j]) continue;

                            int boardY = y + dy + i;
                            int boardX = x + dx + j;

                            // Out of bounds or collides with existing cube
                            if (boardX < 0 || boardX >= ConstData.GameGridColumnCount ||
                                boardY < 0 || boardY >= ConstData.GameGridRowCount ||
                                Board![boardY, boardX].Color != Colors.Transparent)
                            {
                                canPlace = false;
                            }
                        }
                    }

                    if (canPlace)
                    {
                        newX = x + dx;
                        newY = y + dy;
                        return true;
                    }
                }
            }

            return false; // No valid position found
        }
    }
}
