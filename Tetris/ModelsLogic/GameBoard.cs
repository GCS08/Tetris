using Plugin.CloudFirestore;
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
        private async void ShapeAtBottom()
        {
            int linesCleared = CheckForLines();
            if (ShapesQueue!.IsEmpty())
                await fbd.AddShape(new(), GameID!);
        }
        private int CheckForLines()
        {
            int linesCleared = 0;

            int writeRow = ConstData.GameGridRowCount - 1;

            for (int readRow = ConstData.GameGridRowCount - 1; readRow >= 0; readRow--)
            {
                bool lineFull = true;
                for (int col = 0; col < ConstData.GameGridColumnCount; col++)
                {
                    if (Board![readRow, col].Color == Colors.Transparent)
                    {
                        lineFull = false;
                        break;
                    }
                }

                if (!lineFull)
                {
                    if (writeRow != readRow)
                    {
                        for (int col = 0; col < ConstData.GameGridColumnCount; col++)
                        {
                            Board![writeRow, col].Color = Board[readRow, col].Color;
                        }
                    }
                    writeRow--;
                }
                else
                {
                    linesCleared++;
                }
            }

            for (int row = writeRow; row >= 0; row--)
            {
                for (int col = 0; col < ConstData.GameGridColumnCount; col++)
                {
                    Board![row, col].Color = Colors.Transparent;
                }
            }

            return linesCleared;
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
            bool canMoveDown = true;

            int shapeHeight = CurrentShape!.Cells.GetLength(0);
            int shapeWidth = CurrentShape!.Cells.GetLength(1);

            bool[] IsCubesUnderShapeFilled = new bool[shapeWidth];

            // Check collision only if we're not already literally at the last row
            for (int col = 0; col < shapeWidth && canMoveDown; col++)
            {
                bool found = false;
                for (int row = shapeHeight - 1; row >= 0 && !found; row--)
                {
                    // Find the lowest filled block in this column
                    if (CurrentShape.Cells[row, col])
                    {
                        int boardY = CurrentShape.TopLeftY + row + 1;
                        int boardX = CurrentShape.TopLeftX + col;

                        // If stepping down goes outside board -> collision
                        if (boardY >= ConstData.GameGridRowCount)
                            IsCubesUnderShapeFilled[col] = true;
                        else
                        {
                            // Check if the cell under it is filled
                            IsCubesUnderShapeFilled[col] =
                                Board![boardY, boardX].Color != Colors.Transparent;
                        }

                        found = true; // stop scanning this column
                    }
                }

                // If this column is blocked, no point checking the rest
                if (IsCubesUnderShapeFilled[col])
                    canMoveDown = false;
            }

            // Move or lock
            if (canMoveDown)
            {
                EraseShape();
                CurrentShape.TopLeftY++;
                ShowShape();
            }
            else
                ShapeAtBottom();
        }
        private void MoveDownShape(object? sender, ElapsedEventArgs e)
        {
            MoveDownShape();
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
            int[] shiftsX = [0, -1, 1, -2, 2];
            int[] shiftsY = [0, -1, 1];

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
        public void AddListener()
        {
            ilr = fbd.AddGameBoardListener(GameID!, OnChange!);
        }
        public void RemoveListener()
        {
            ilr?.Remove();
        }
        private void OnChange(IDocumentSnapshot snapshot, Exception error)
        {
            ShapesQueue!.Insert(new Shape(
                snapshot.Get<int>(Keys.CurrentShapeIdKey)!,
                snapshot.Get<string>(Keys.CurrentShapeColorKey)!
            ));
            CurrentShape = ShapesQueue.Remove();
            ShowShape();
        }
    }
}
