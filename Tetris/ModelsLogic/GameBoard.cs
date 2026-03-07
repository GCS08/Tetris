using System.Timers;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    /// <summary>
    /// Represents the game board for a Tetris-like game, managing the state of the grid, 
    /// current and queued shapes, player moves, and game logic such as moving, rotating, 
    /// and locking shapes, clearing lines, and detecting game over conditions.
    /// </summary>
    /// <remarks>
    /// This class handles both local game state and integrates with the backend for player-specific 
    /// actions when not in opponent mode. It provides methods for moving and rotating shapes, 
    /// checking for completed lines, updating scores, and notifying when the game is finished.
    /// </remarks>
    public class GameBoard : GameBoardModel
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GameBoard"/> class with the specified game ID,
        /// current shape, and opponent mode. Sets up the game grid, initializes cubes, and configures
        /// the fall timer for automatic shape movement.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <param name="currentShape">The shape that will start on the board.</param>
        /// <param name="IsOp">Indicates whether the board is in opponent mode, affecting grid sizing and behavior.</param>
        public GameBoard(string gameId, Shape currentShape, bool IsOp)
        {
            Board = new Cube[ConstData.GameGridRowCount, ConstData.GameGridColumnCount];
            this.CurrentShape = currentShape;
            this.IsOp = IsOp;
            this.GameID = gameId;

            FallTimer = Application.Current!.Dispatcher.CreateTimer();
            FallTimer.Interval = TimeSpan.FromSeconds(ConstData.ShapeFallIntervalS);
            FallTimer.Tick += async (s, e) => await MoveDownShape();

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
        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the game by enabling player moves and, if debugging is enabled,
        /// starting the automatic fall timer for shapes.
        /// </summary>
        public override void StartGame()
        {
            if (ConstData.DebugData.StartFallTimer && FallTimer != null)
                FallTimer.Start();
            EnableMoves = true;
        }

        /// <summary>
        /// Attempts to move the current shape one column to the right on the game board,
        /// checking for collisions with the board boundaries or other placed cubes.
        /// If movement is possible, updates the shape's position and records the move.
        /// </summary>
        public override void MoveRightShape()
        {
            if (!EnableMoves || CurrentShape == null || Board == null || MovesQueue == null) return;

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
                MovesQueue.Insert(Keys.RightKey);
            }
        }

        /// <summary>
        /// Attempts to move the current shape one column to the left on the game board,
        /// checking for collisions with the board boundaries or other placed cubes.
        /// If movement is possible, updates the shape's position and records the move.
        /// </summary>
        public override void MoveLeftShape()
        {
            if (!EnableMoves || CurrentShape == null || Board == null || MovesQueue == null) return;

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
                MovesQueue.Insert(Keys.LeftKey);
            }
        }

        /// <summary>
        /// Attempts to move the current shape one row down on the game board, checking for collisions
        /// with the bottom of the board or other placed cubes. If the shape cannot move further,
        /// it locks the shape in place and triggers end-of-round logic for the player if applicable.
        /// </summary>
        /// <returns>A task representing the asynchronous operation of moving the shape down.</returns>
        public override async Task MoveDownShape() 
        {
            if (CurrentShape == null || Board == null || (!IsOp && User == null) || GameID == null || MovesQueue == null) return;

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
            
            if (EnableMoves)
            {
                MovesQueue.Insert(Keys.DownKey);
                // Move or lock
                if (canMoveDown)
                {
                    EraseShape();
                    CurrentShape.TopLeftY++;
                    ShowShape();
                    if (!IsOp)
                        SoundManager.PlayMoveDown();
                }
                else
                    ShapeAtBottom();
            }
        }

        /// <summary>
        /// Rotates the current shape to its next rotation state if possible, checking for collisions
        /// with the board boundaries and other placed cubes. Updates the shape's position accordingly
        /// and records the rotation move.
        /// </summary>
        public override void RotateShape()
        {
            if (CurrentShape == null || CurrentShape.RotationStates == null
                || !EnableMoves || MovesQueue == null) return;

            int nextIndex = (CurrentShape.RotationIndex + 1) % CurrentShape.RotationStates.Count;
            bool[,] nextCells = CurrentShape.RotationStates[nextIndex];

            EraseShape();

            // Try rotation at the original position first, then small shifts
            if (TryPlaceRotation(nextCells, CurrentShape.TopLeftX, 
                CurrentShape.TopLeftY, out int newX, out int newY))
            {
                CurrentShape.TopLeftX = newX;
                CurrentShape.TopLeftY = newY;
                CurrentShape.RotationIndex = nextIndex;
            }

            MovesQueue.Insert(Keys.RotateKey);
            ShowShape();
        }

        /// <summary>
        /// Sets up the visual representation of the game board by initializing the grid with rows and columns,
        /// creating cube views for each cell, and binding cube color changes to the UI elements.
        /// </summary>
        /// <param name="gameBoardGrid">The <see cref="Grid"/> control to initialize with the game board cells.</param>
        public override void InitializeGrid(Grid? gameBoardGrid)
        {
            if (gameBoardGrid == null || Board == null) return;

            bool initialized = false;

            gameBoardGrid.SizeChanged += (s, e) =>
            {
                if (initialized) return;
                if (gameBoardGrid.Width <= 0 || gameBoardGrid.Height <= 0) return;

                initialized = true;

                double cubeSize = Math.Min(
                    gameBoardGrid.Width / ConstData.GameGridColumnCount,
                    gameBoardGrid.Height / ConstData.GameGridRowCount
                );

                for (int r = 0; r < ConstData.GameGridRowCount; r++)
                    gameBoardGrid.RowDefinitions.Add(new RowDefinition { Height = cubeSize });

                for (int c = 0; c < ConstData.GameGridColumnCount; c++)
                    gameBoardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = cubeSize });

                for (int r = 0; r < ConstData.GameGridRowCount; r++)
                {
                    for (int c = 0; c < ConstData.GameGridColumnCount; c++)
                    {
                        Cube cube = Board[r, c];

                        BoxView boxView = new()
                        {
                            BackgroundColor = cube.Color
                        };

                        cube.PropertyChanged += (s2, e2) =>
                        {
                            if (e2.PropertyName == nameof(Cube.Color))
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

                gameBoardGrid.HorizontalOptions = LayoutOptions.Center;
                gameBoardGrid.VerticalOptions = LayoutOptions.Center;
            };
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Handles the logic when the current shape reaches the bottom of the board,
        /// including clearing completed lines, updating the score and combo count,
        /// checking for game over, and preparing the next shape from the queue.
        /// </summary>
        protected override async void ShapeAtBottom()
        {
            if (User == null || GameID == null || MovesQueue == null) return;

            int linesCleared = CheckForLines();
            if (!IsOp)
                await fbd.FinishRound(User!.UserID, GameID, MovesQueue);
            if (linesCleared > 0)
            {
                if (!IsOp)
                    SoundManager.PlayLineCleared();
                User.TotalLines += linesCleared;
                Score += linesCleared * ConstData.ScorePerLine * ComboCount;
                ComboCount++;
            }
            else
                ComboCount = 1;

            if (CheckForLose())
                OnGameFinishedLogic?.Invoke(this, EventArgs.Empty);
            else
            {
                if (ShapesQueue == null || CurrentShape == null || FallTimer == null) return;

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
                if (!IsOp)
                    FallTimer.Interval = TimeSpan.FromSeconds(ConstData.ShapeFallIntervalS - CurrentShape.InGameId * 0.03);
                ShowShape();
            }
        }

        /// <summary>
        /// Checks whether the game has been lost by verifying if any cells in the top row of the board are filled.
        /// </summary>
        /// <returns>True if the game is over (top row has filled cells); otherwise, false.</returns>
        protected override bool CheckForLose()
        {
            bool result = false;
            if (Board != null)
            {
                int col = 0;
                while (col < ConstData.GameGridColumnCount && !result)
                {
                    if (Board[0, col].Color != Colors.Transparent)
                        result = true;
                    col++;
                }
            }
            return result;
        }

        /// <summary>
        /// Checks the game board for fully filled lines, clears them, shifts down remaining lines,
        /// and returns the number of lines that were cleared.
        /// </summary>
        /// <returns>The number of lines cleared from the board.</returns>
        protected override int CheckForLines()
        {
            if (Board == null) return 0;
            int linesCleared = 0;

            int writeRow = ConstData.GameGridRowCount - 1;

            for (int readRow = ConstData.GameGridRowCount - 1; readRow >= 0; readRow--)
            {
                bool lineFull = true;
                for (int col = 0; col < ConstData.GameGridColumnCount && lineFull; col++)
                    if (Board[readRow, col].Color == Colors.Transparent)
                        lineFull = false;

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

        /// <summary>
        /// Renders the current shape onto the game board by updating the colors of the corresponding cubes
        /// based on the shape's position and filled cells.
        /// </summary>
        protected override void ShowShape()
        {
            if (CurrentShape == null || Board == null) return;
            for (int i = 0; i < CurrentShape.Cells.GetLength(0); i++)
                for (int j = 0; j < CurrentShape.Cells.GetLength(1); j++)
                    if (CurrentShape.Cells[i, j])
                        Board[i + CurrentShape.TopLeftY, j +
                            CurrentShape.TopLeftX].Color = CurrentShape.Color;
        }

        /// <summary>
        /// Removes the current shape from the game board by setting the colors of its occupied cubes to transparent.
        /// </summary>
        protected override void EraseShape()
        {
            if (CurrentShape == null || Board == null) return;
            for (int i = 0; i < CurrentShape.Cells.GetLength(0); i++)
                for (int j = 0; j < CurrentShape.Cells.GetLength(1); j++)
                    if (CurrentShape.Cells[i, j])
                        Board[i + CurrentShape.TopLeftY, j +
                            CurrentShape.TopLeftX].Color = Colors.Transparent;
        }

        /// <summary>
        /// Event handler that triggers the asynchronous downward movement of the current shape
        /// when the fall timer elapses.
        /// </summary>
        /// <param name="sender">The source of the event (typically the timer).</param>
        /// <param name="e">The event arguments associated with the timer tick.</param>
        protected override async void MoveDownShape(object? sender, ElapsedEventArgs e) 
        {
            if (GameID == null) return;
            await MoveDownShape();
        }

        /// <summary>
        /// Attempts to place a rotated shape on the board, applying small position adjustments
        /// (wall kicks) if necessary to avoid collisions with boundaries or other cubes.
        /// </summary>
        /// <param name="cells">The cell layout of the rotated shape.</param>
        /// <param name="x">The initial X-coordinate on the board.</param>
        /// <param name="y">The initial Y-coordinate on the board.</param>
        /// <param name="newX">Outputs the adjusted X-coordinate where the shape can be placed.</param>
        /// <param name="newY">Outputs the adjusted Y-coordinate where the shape can be placed.</param>
        /// <returns>True if a valid position was found for the rotated shape; otherwise, false.</returns>
        protected override bool TryPlaceRotation(
            bool[,] cells, int x, int y,
            out int newX, out int newY)
        {
            bool result = false;

            newX = x;
            newY = y;

            if (Board != null)
            {
                int h = cells.GetLength(0);
                int w = cells.GetLength(1);

                int[] shiftsX = [0, -1, 1, -2, 2];
                int[] shiftsY = [0, -1, 1];

                int sx = 0;
                while (sx < shiftsX.Length && !result)
                {
                    int sy = 0;
                    while (sy < shiftsY.Length && !result)
                    {
                        int dx = shiftsX[sx];
                        int dy = shiftsY[sy];
                        bool canPlace = true;
                        int i = 0;
                        while (i < h && canPlace)
                        {
                            int j = 0;
                            while (j < w && canPlace)
                            {
                                if (cells[i, j])
                                {
                                    int boardY = y + dy + i;
                                    int boardX = x + dx + j;
                                    if (boardX < 0 || boardX >= ConstData.GameGridColumnCount ||
                                        boardY < 0 || boardY >= ConstData.GameGridRowCount ||
                                        Board[boardY, boardX].Color != Colors.Transparent)
                                        canPlace = false;
                                }
                                j++;
                            }
                            i++;
                        }

                        if (canPlace)
                        {
                            newX = x + dx;
                            newY = y + dy;
                            result = true;
                        }
                        sy++;
                    }
                    sx++;
                }
            }

            return result;
        }
        #endregion
    }
}
