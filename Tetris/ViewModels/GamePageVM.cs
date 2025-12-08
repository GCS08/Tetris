using Tetris.Models;
using Tetris.ModelsLogic;
using System.Windows.Input;

namespace Tetris.ViewModels;

public partial class GamePageVM(Game game) : ObservableObject
{
    public GridLength UserScreenHeight => ConstData.UserScreenHeight;

    public ICommand MoveRightShapeCommand => new Command(MoveRightShape);
    public ICommand MoveLeftShapeCommand => new Command(MoveLeftShape);
    public ICommand MoveDownShapeCommand => new Command(MoveDownShape);
    public ICommand RotateShapeCommand => new Command(RotateShape);

    public Game CurrentGame { get; } = game;
    public Grid? GameBoardGrid { get; set; }
    public Grid? OpGameBoardGrid { get; set; }

    private void MoveRightShape()
    {
        CurrentGame.MoveRightShape();
    }
    private void MoveLeftShape()
    {
        CurrentGame.MoveLeftShape();
    }
    private void MoveDownShape()
    {
        CurrentGame.MoveDownShape();
    }
    private void RotateShape()
    {
        CurrentGame.RotateShape();
    }
    public void InitializeGrid()
    {
        CurrentGame.GameBoard!.InitializeGrid(GameBoardGrid, ConstData.GameGridColumnWidth, ConstData.GameGridRowHeight);
        CurrentGame.OpGameBoard!.InitializeGrid(OpGameBoardGrid, ConstData.OpGameGridColumnWidth, ConstData.OpGameGridRowHeight);
    }

    public void AddListener()
    {
        CurrentGame.GameBoard!.AddListener();
    }

    public void RemoveListener()
    {
        CurrentGame.GameBoard!.RemoveListener();
    }
}
