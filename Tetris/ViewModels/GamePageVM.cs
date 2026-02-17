using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels;

public partial class GamePageVM : ObservableObject
{
    public bool IsReadyVisible { get; set; } = true;
    public bool IsTimerVisible { get; set; } = false;
    public bool IsGameFinishedVisible { get; set; } = false;

    public Color GameFinishedResultColor { get; set; } = Colors.Black;
    public string GameFinishedResultText { get; set; } = Strings.LoadingResult;
    public string TimeLeft => CurrentGame.TimeLeftText;
    public string OpName => CurrentGame.OpGameBoard?.User?.UserName ?? Strings.UaUsername;
    public string PlayerName => (Application.Current as App)!.AppUser.UserName;
    public GridLength UserScreenHeight => ConstData.UserScreenHeight;

    public ICommand NavToGameLobbyCommand => new Command(NavToGameLobby);
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
        if (game.GameBoard == null || game.OpGameBoard == null) return;
        game.OnAllReady += OnAllReadyHandler;
        game.OnTimeLeftChanged += OnTimeLeftChangedHandler;
        game.OnGameFinishedUI += OnGameFinishedUIHandler;
    }

    private void OnGameFinishedUIHandler(object? sender, EventArgs e)
    {
        bool isOpLost = (sender as GameBoard).IsOp;
        IsGameFinishedVisible = true;
        GameFinishedResultColor = isOpLost ? Colors.Green : Colors.Red;
        GameFinishedResultText = isOpLost ? Strings.YouWon : Strings.YouLost;
        OnPropertyChanged(nameof(IsGameFinishedVisible));
        OnPropertyChanged(nameof(GameFinishedResultColor));
        OnPropertyChanged(nameof(GameFinishedResultText));
    }
    private void OnTimeLeftChangedHandler(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(TimeLeft));

        if (CurrentGame.TimeLeftMs <= 0)
        {
            IsTimerVisible = false;
            OnPropertyChanged(nameof(IsTimerVisible));
            CurrentGame.StartGame();
        }
    }

    private void OnAllReadyHandler(object? sender, EventArgs e)
    {
        IsReadyVisible = false;
        IsTimerVisible = true;

        OnPropertyChanged(nameof(IsReadyVisible));
        OnPropertyChanged(nameof(IsTimerVisible));

        CurrentGame.PrepareGame();
    }

    private void NavToGameLobby()
    {
        _ = Shell.Current.Navigation.PushAsync(new GameLobbyPage());
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
        if (CurrentGame.GameBoard == null || CurrentGame.OpGameBoard == null || GameBoardGrid == null || OpGameBoardGrid == null) return;
        CurrentGame.GameBoard.InitializeGrid(GameBoardGrid, ConstData.GameGridColumnWidth, ConstData.GameGridRowHeight);
        CurrentGame.OpGameBoard.InitializeGrid(OpGameBoardGrid, ConstData.OpGameGridColumnWidth, ConstData.OpGameGridRowHeight);
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
