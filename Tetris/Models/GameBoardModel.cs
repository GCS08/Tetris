using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class GameBoardModel
    {
        public Cube[,]? Board;
        public Shape? CurrentShape { get; set; }
        public ModelsLogic.Queue<Shape>? ShapesQueue { get; set; } = new();
        protected System.Timers.Timer FallTimer = new(ConstData.SecondsTillShapeFall * 1000);
        public abstract void ShowShape();
        public abstract void MoveRightShape();
        public abstract void MoveLeftShape();
        public abstract void MoveDownShape();
        public abstract void RotateShape();
    }
}
