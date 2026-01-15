using Plugin.CloudFirestore;
using System.Timers;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class GameBoard : GameBoardModel
    {
        public GameBoard(string gameId, Shape currentShape, bool IsOp)
        {
            Board = new Cube[ConstData.GameGridRowCount, ConstData.GameGridColumnCount];
            this.CurrentShape = currentShape;
            this.IsOp = IsOp;
            this.GameID = gameId;
            
            double cubeWidth = IsOp ? ConstData.OpGameGridColumnWidth : ConstData.GameGridColumnWidth;
            double cubeHeight = IsOp ? ConstData.OpGameGridRowHeight : ConstData.GameGridRowHeight;

            for (int r = 0; r < ConstData.GameGridRowCount; r++)
            {
                for (int c = 0; c < ConstData.GameGridColumnCount; c++)
                {
                    Board[r, c] = new Cube(
                        cubeWidth,
                        cubeHeight,
                        Colors.Transparent
                    );
                }
            }
        }
        public void StartGame()
        {
            if (!IsOp)
            {
                FallTimer.Elapsed += MoveDownShape;
                FallTimer.Start();
            }
        }
        public override void ShowShape()
        {
            if (CurrentShape == null || Board == null) return;
            for (int i = 0; i < CurrentShape.Cells.GetLength(0); i++)
                for (int j = 0; j < CurrentShape.Cells.GetLength(1); j++)
                    if (CurrentShape.Cells[i, j])
                        Board[i + CurrentShape.TopLeftY, j +
                            CurrentShape.TopLeftX].Color = CurrentShape.Color;
         }
        private void ShapeAtBottom()
        {
            int linesCleared = CheckForLines();
            if (CheckForLose())
            {
                IsLost = true;
                FallTimer.Stop();
            }
            else
            {
                if (ShapesQueue == null || CurrentShape == null || GameID == null) return;
                
                ShapesQueue.Remove();
                if (ShapesQueue.IsEmpty() && !IsOp)
                {
                    Shape shape = new(CurrentShape.InGameId + 1);
                    fbd.AddShape(shape, GameID);
                    ShapesQueue.Insert(shape);
                    CurrentShape = shape;
                }
                else
                    CurrentShape = ShapesQueue.Head();
                ShowShape();
            }
        }
        private bool CheckForLose()
        {
            if (Board == null) return false;
            for (int col = 0; col < ConstData.GameGridColumnCount; col++)
                if (Board[0, col].Color != Colors.Transparent)
                    return true;
            return false;
        }
        private int CheckForLines()
        {
            if (Board == null) return 0;
            int linesCleared = 0;

            int writeRow = ConstData.GameGridRowCount - 1;

            for (int readRow = ConstData.GameGridRowCount - 1; readRow >= 0; readRow--)
            {
                bool lineFull = true;
                for (int col = 0; col < ConstData.GameGridColumnCount; col++)
                    if (Board[readRow, col].Color == Colors.Transparent)
                    {
                        lineFull = false;
                        break;
                    }

                if (!lineFull)
                {
                    if (writeRow != readRow)
                        for (int col = 0; col < ConstData.GameGridColumnCount; col++)
                            Board[writeRow, col].Color = Board[readRow, col].Color;
                    writeRow--;
                }
                else
                    linesCleared++;
            }

            for (int row = writeRow; row >= 0; row--)
                for (int col = 0; col < ConstData.GameGridColumnCount; col++)
                    Board[row, col].Color = Colors.Transparent;

            return linesCleared;
        }
        private void EraseShape()
        {
            if (CurrentShape == null || Board == null) return;

            for (int i = 0; i < CurrentShape.Cells.GetLength(0); i++)
                for (int j = 0; j < CurrentShape.Cells.GetLength(1); j++)
                    if (CurrentShape.Cells[i, j])
                        Board[i + CurrentShape.TopLeftY, j +
                            CurrentShape.TopLeftX].Color = Colors.Transparent;
        }
        public override void MoveRightShape()
        {
            if (IsLost || CurrentShape == null || Board == null)
                return;

            int shapeHeight = CurrentShape.Cells.GetLength(0);
            int shapeWidth = CurrentShape.Cells.GetLength(1);

            bool canMoveRight = true;

            // Check collision on the right
            for (int row = 0; row < shapeHeight && canMoveRight; row++)
            {
                bool found = false;
                for (int col = shapeWidth - 1; col >= 0 && !found; col--)
                {
                    // Find the rightmost filled block in this row
                    if (CurrentShape.Cells[row, col])
                    {
                        int boardY = CurrentShape.TopLeftY + row;
                        int boardX = CurrentShape.TopLeftX + col + 1;

                        // Outside board
                        if (boardX >= ConstData.GameGridColumnCount)
                        {
                            canMoveRight = false;
                        }
                        else
                        {
                            // Check board collision
                            if (Board[boardY, boardX].Color != Colors.Transparent)
                                canMoveRight = false;
                        }

                        found = true;
                    }
                }
            }

            if (canMoveRight)
            {
                EraseShape();
                CurrentShape.TopLeftX++;
                ShowShape();
            }
        }
        public override void MoveLeftShape()
        {
            if (IsLost || CurrentShape == null || Board == null)
                return;

            int shapeHeight = CurrentShape.Cells.GetLength(0);
            int shapeWidth = CurrentShape.Cells.GetLength(1);

            bool canMoveLeft = true;

            // Check collision on the left
            for (int row = 0; row < shapeHeight && canMoveLeft; row++)
            {
                bool found = false;
                for (int col = 0; col < shapeWidth && !found; col++)
                {
                    // Find the leftmost filled block in this row
                    if (CurrentShape.Cells[row, col])
                    {
                        int boardY = CurrentShape.TopLeftY + row;
                        int boardX = CurrentShape.TopLeftX + col - 1;

                        // Outside board
                        if (boardX < 0)
                        {
                            canMoveLeft = false;
                        }
                        else
                        {
                            // Check board collision
                            if (Board[boardY, boardX].Color != Colors.Transparent)
                                canMoveLeft = false;
                        }

                        found = true;
                    }
                }
            }

            if (canMoveLeft)
            {
                EraseShape();
                CurrentShape.TopLeftX--;
                ShowShape();
            }
        }
        public override async Task<bool> MoveDownShape() 
        {
            if (CurrentShape == null || Board == null || (!IsOp && User == null) || GameID == null)
                return false;

            bool canMoveDown = true;

            int shapeHeight = CurrentShape.Cells.GetLength(0);
            int shapeWidth = CurrentShape.Cells.GetLength(1);

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
                                Board[boardY, boardX].Color != Colors.Transparent;
                        }

                        found = true; // stop scanning this column
                    }
                }

                // If this column is blocked, no point checking the rest
                if (IsCubesUnderShapeFilled[col])
                    canMoveDown = false;
            }
            
            bool isAtBottom = false;
            if (!IsLost)
            {
                // Move or lock
                if (canMoveDown)
                {
                    EraseShape();
                    CurrentShape.TopLeftY++;
                    ShowShape();
                }
                else
                {
                    isAtBottom = true;
                    ShapeAtBottom();
                    if (!IsOp)
                        await fbd.PlayerActionWithBottom(User!.UserID, GameID, Keys.DownKey);
                }
            }
            return isAtBottom;
        }
        private async void MoveDownShape(object? sender, ElapsedEventArgs e) 
        {
            if (GameID == null) return;
            bool isAtBottom = await MoveDownShape();
            if (!isAtBottom)
                await fbd.PlayerAction(GameID, (Application.Current as App)!.user.UserID, Keys.DownKey);
        }
        public override void RotateShape()
        {
            if (CurrentShape == null || CurrentShape.RotationStates == null || IsLost) return;

            int nextIndex = (CurrentShape.RotationIndex + 1) % CurrentShape.RotationStates.Count;
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
            if (Board == null)
            {
                newX = x;
                newY = y;
                return false;
            }

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
                                Board[boardY, boardX].Color != Colors.Transparent)
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
        public void InitializeGrid(Grid? gameBoardGrid, double cubeWidth, double cubeHeight)
        {
            if (gameBoardGrid == null || Board == null) return;

            for (int r = 0; r < ConstData.GameGridRowCount; r++)
                gameBoardGrid.RowDefinitions.Add(new RowDefinition { Height = cubeHeight });

            for (int c = 0; c < ConstData.GameGridColumnCount; c++)
                gameBoardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = cubeWidth });

            for (int r = 0; r < ConstData.GameGridRowCount; r++)
            {
                for (int c = 0; c < ConstData.GameGridColumnCount; c++)
                {
                    Cube cube = Board[r, c];

                    BoxView boxView = new()
                    {
                        WidthRequest = cube.Width,
                        HeightRequest = cube.Height,
                        BackgroundColor = cube.Color
                    };

                    cube.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(cube.Color))
                            boxView.BackgroundColor = cube.Color;
                    };

                    Border border = new()
                    {
                        Margin = -0.5 * ConstData.BetweenCubesBorderWidth,
                        Stroke = Colors.Gray,
                        StrokeThickness = ConstData.BetweenCubesBorderWidth,
                        Background = Colors.Transparent,
                        Content = boxView
                    };

                    gameBoardGrid.Add(border, c, r);
                }
            }
        }
    }
}
