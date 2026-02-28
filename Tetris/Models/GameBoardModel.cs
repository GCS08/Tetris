using Microsoft.Maui;
using Tetris.Interfaces;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class GameBoardModel
    {
        public Cube[,]? Board;
        public Shape? CurrentShape { get; set; }
        public string? GameID;
        public ModelsLogic.Queue<Shape>? ShapesQueue { get; set; } = new();
        public ModelsLogic.Queue<string>? MovesQueue { get; set; } = new();
        public User? User { get; set; }
        public bool IsOp { get; set; }
        public IDispatcherTimer? FallTimer;
        protected FbData fbd = IPlatformApplication.Current?.Services.GetService<IFbData>() as FbData ?? new();
        public EventHandler? OnOpQueueEmpty;
        public EventHandler? OnGameFinishedLogic;
        public bool EnableMoves { get; set; } = false;
        public int Score { get; set; } = 0;
        protected int ComboCount { get; set; } = 1;
        protected abstract void ShowShape();
        protected abstract void EraseShape();
        public abstract void MoveRightShape();
        public abstract void MoveLeftShape();
        public abstract Task MoveDownShape();
        public abstract void RotateShape();
    }
}
