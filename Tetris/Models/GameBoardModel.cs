using System.Timers;
using Microsoft.Maui;
using Tetris.Interfaces;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class GameBoardModel
    {
        public Shape? CurrentShape { get; set; }
        public string? GameID;
        public ModelsLogic.Queue<Shape>? ShapesQueue { get; set; } = new();
        public User? User { get; set; }
        public bool IsOp { get; set; }
        public IDispatcherTimer? FallTimer;
        public EventHandler? OnOpQueueEmpty;
        public EventHandler? OnGameFinishedLogic;
        public bool EnableMoves { get; set; } = false;
        public int Score { get; set; } = 0;
        protected FbData fbd = IPlatformApplication.
            Current?.Services.GetService<IFbData>() as FbData ?? new();
        protected ModelsLogic.Queue<string>? MovesQueue { get; set; } = new();
        protected Cube[,]? Board;
        protected SoundManager SoundManager = IPlatformApplication.
            Current?.Services.GetService<ISoundManager>() as SoundManager ?? new();
        protected int ComboCount { get; set; } = 1;
        public abstract void StartGame();
        protected abstract void ShapeAtBottom();
        protected abstract bool CheckForLose();
        protected abstract void ShowShape();
        protected abstract int CheckForLines();
        protected abstract void MoveDownShape(object? sender, ElapsedEventArgs e);
        protected abstract bool TryPlaceRotation(bool[,] cells, int x, int y, out int newX, out int newY);
        public abstract void InitializeGrid(Grid? gameBoardGrid, double cubeWidth, double cubeHeight);
        protected abstract void EraseShape();
        public abstract void MoveRightShape();
        public abstract void MoveLeftShape();
        public abstract Task MoveDownShape();
        public abstract void RotateShape();
    }
}
