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
        protected System.Timers.Timer FallTimer = new(ConstData.ShapeFallIntervalS * 1000);
        protected FbData fbd = new();
        public EventHandler? OnOpQueueEmpty;
        public EventHandler? OnGameFinished;
        public bool IsLost = false;
        protected abstract void ShowShape();
        protected abstract void EraseShape();
        public abstract void MoveRightShape();
        public abstract void MoveLeftShape();
        public abstract Task MoveDownShape();
        public abstract void RotateShape();
    }
}
