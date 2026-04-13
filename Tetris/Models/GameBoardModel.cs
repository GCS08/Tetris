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
        public IDispatcherTimer? FallTimer;
        protected IDispatcherTimer? PushMovesToFirebaseTimer;
        protected FbData fbd = IPlatformApplication.
            Current?.Services.GetService<IFbData>() as FbData ?? new();
        protected SoundManager SoundManager = IPlatformApplication.
            Current?.Services.GetService<ISoundManager>() as SoundManager ?? new();
        protected bool isMoving = false;
        #endregion

        #region Events
        public EventHandler? OnGameFinishedLogic;
        #endregion

        #region Properties
        public Shape? CurrentShape { get; set; }
        public string? GameID { get; set; }
        public ModelsLogic.Queue<Shape> ShapesQueue { get; set; } = new();
        public User? User { get; set; }
        public bool IsOp { get; set; }
        public bool EnableMoves { get; set; } = false;
        public int Score { get; set; } = 0;
        protected ModelsLogic.Queue<Dictionary<string, int>> QueueOfFinalStates { get; set; } = new();
        protected int ComboCount { get; set; } = 1;
        protected Cube[,]? Board { get; set; }
        #endregion

        #region Public Methods
        public abstract void StartGame();
        public abstract void InitializeGrid(Grid? gameBoardGrid);
        public abstract void MoveRightShape();
        public abstract void MoveLeftShape();
        public abstract void MoveDownShape();
        public abstract void SnapDownShape();
        public abstract void RotateShape();
        public abstract void ShowShape();
        public abstract void ApplyMovesFromMap(Dictionary<string, int> playerMoveMap);
        #endregion

        #region Protected Methods
        protected abstract bool CanMoveDown();
        protected abstract void ShapeAtBottom();
        protected abstract void PushMovesToFirebase();
        protected abstract bool CheckForLose();
        protected abstract int CheckForLines();
        protected abstract void MoveDownShape(object? sender, ElapsedEventArgs e);
        protected abstract bool TryPlaceRotation(bool[,] cells, int x, int y, out int newX, out int newY);
        protected abstract void EraseShape();
        #endregion
    }
}
