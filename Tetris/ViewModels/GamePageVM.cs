using Tetris.Models;
using Tetris.ModelsLogic;
using System.Windows.Input;

namespace Tetris.ViewModels;

public partial class GamePageVM : ObservableObject
{
    public bool IsReadyVisible { get; set; } = true;
    public string TimeLeft => CurrentGame.TimeLeft;
    public GridLength UserScreenHeight => ConstData.UserScreenHeight;

    public ICommand ReadyCommand => new Command(Ready);
    public ICommand MoveRightShapeCommand => new Command(MoveRightShape);
    public ICommand MoveLeftShapeCommand => new Command(MoveLeftShape);
    public ICommand MoveDownShapeCommand => new Command(MoveDownShape);
    public ICommand RotateShapeCommand => new Command(RotateShape);

    public Game CurrentGame { get; }
    public Grid? GameBoardGrid { get; set; }
    public Grid? OpGameBoardGrid { get; set; }

    public GamePageVM(Game game)
    {
        this.CurrentGame = game;
        game.OnAllReady += OnAllReadyHandler;
        game.OnTimeLeftChanged += OnTimeLeftChangedHandler;
    }

    private void OnTimeLeftChangedHandler(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(TimeLeft));
    }

    private void OnAllReadyHandler(object? sender, EventArgs e)
    {
        IsReadyVisible = false;
        OnPropertyChanged(nameof(IsReadyVisible));
        CurrentGame.StartGame();
    }

    private void Ready()
    {
        CurrentGame.Ready();
    }
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

    public void AddReadyListener()
    {
        CurrentGame.AddReadyListener();
    }

    public void RemoveGameListener()
    {
        CurrentGame.RemoveGameListener();
    }
}
