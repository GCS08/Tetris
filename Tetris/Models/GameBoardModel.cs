using System.Timers;
using Tetris.Interfaces;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    /// <summary>
    /// Represents the abstract base class for a game board, managing the state, logic, and interactions for a
    /// grid-based game involving shapes, scoring, and user actions.
    /// </summary>
    public abstract class GameBoardModel
    {
        #region Fields
        protected Cube[,]? Board;
        protected FbData fbd = IPlatformApplication.
            Current?.Services.GetService<IFbData>() as FbData ?? new();
        protected SoundManager SoundManager = IPlatformApplication.
            Current?.Services.GetService<ISoundManager>() as SoundManager ?? new();
        public IDispatcherTimer? FallTimer;
        #endregion

        #region Events
        public EventHandler? OnOpQueueEmpty;
        public EventHandler? OnGameFinishedLogic;
        #endregion

        #region Properties
        public Shape? CurrentShape { get; set; }
        public string? GameID { get; set; }
        public ModelsLogic.Queue<Shape>? ShapesQueue { get; set; } = new();
        public User? User { get; set; }
        public bool IsOp { get; set; }
        public bool EnableMoves { get; set; } = false;
        public int Score { get; set; } = 0;
        protected ModelsLogic.Queue<string>? MovesQueue { get; set; } = new();
        protected int ComboCount { get; set; } = 1;
        #endregion

        #region Public Methods
        public abstract void StartGame();
        public abstract void InitializeGrid(Grid? gameBoardGrid, double cubeWidth, double cubeHeight);
        public abstract void MoveRightShape();
        public abstract void MoveLeftShape();
        public abstract Task MoveDownShape();
        public abstract void RotateShape();
        #endregion

        #region Protected Methods
        protected abstract void ShapeAtBottom();
        protected abstract bool CheckForLose();
        protected abstract void ShowShape();
        protected abstract int CheckForLines();
        protected abstract void MoveDownShape(object? sender, ElapsedEventArgs e);
        protected abstract bool TryPlaceRotation(bool[,] cells, int x, int y, out int newX, out int newY);
        protected abstract void EraseShape();
        #endregion
    }
}
