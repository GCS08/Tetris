using Plugin.CloudFirestore;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class GameBoardModel
    {
        public Cube[,]? Board;
        public Shape? CurrentShape { get; set; }
        public string? GameID;
        public ModelsLogic.Queue<Shape>? ShapesQueue { get; set; } = new();
        public User? User { get; set; }
        public bool IsOp { get; set; }
        protected System.Timers.Timer FallTimer = new(ConstData.ShapeFallInternalS * 1000);
        protected FbData fbd = new();
        public EventHandler? OnOpQueueEmpty;
        protected bool IsLost = false;
        public abstract void ShowShape();
        public abstract void MoveRightShape();
        public abstract void MoveLeftShape();
        public abstract Task<bool> MoveDownShape(Dictionary<string, object> shapeData = null!);
        public abstract void RotateShape();
    }
}
