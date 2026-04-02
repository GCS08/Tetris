using System.Windows.Input;
using Tetris.Interfaces;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels;

/// <summary>
/// ViewModel for the Game page.
/// Handles user interaction, game state updates, and binding to the UI elements 
/// such as game boards, timers, and finished game notifications.
/// </summary>
public partial class GamePageVM : ObservableObject
{
    #region Fields
    public User User = IPlatformApplication.Current?.Services.GetService<IUser>() as User ?? new();
    private readonly ModelsLogic.Connectivity connectivity = new();
    #endregion

    #region ICommands
    public ICommand NavToGameLobbyCommand => new Command(NavToGameLobby);
    public ICommand ReadyCommand => new Command(Ready);
    public ICommand MoveRightShapeCommand => new Command(MoveRightShape);
    public ICommand MoveLeftShapeCommand => new Command(MoveLeftShape);
    public ICommand MoveDownShapeCommand => new Command(MoveDownShape);
    public ICommand SnapDownShapeCommand => new Command(SnapDownShape);
    public ICommand RotateShapeCommand => new Command(RotateShape);
    #endregion

    #region Properties
    public bool IsReadyVisible { get; set; } = true;
    public bool IsTimerVisible { get; set; } = false;
    public bool IsGameFinishedVisible { get; set; } = false;
    public bool IsNoInternetVisible => !connectivity.IsConnected;

    public Color GameFinishedResultColor { get; set; } = Colors.Black;
    public string GameFinishedResultText { get; set; } = Strings.LoadingResult;
    public string TimeLeft => CurrentGame.TimeLeftText;
    public string OpName => CurrentGame.OpGameBoard?.User?.UserName ?? Strings.UsernameUa;
    public string PlayerName => User?.UserName ?? Strings.UsernameUa;

    public Game CurrentGame { get; }
    public Grid? GameBoardGrid { get; set; }
    public Grid? OpGameBoardGrid { get; set; }
    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="GamePageVM"/> with a specific game.
    /// Registers event handlers for game readiness, timer updates, and game finish UI.
    /// </summary>
    /// <param name="game">The game instance to bind to this ViewModel.</param>
    public GamePageVM(Game game)
    {
        this.CurrentGame = game;
        if (game.GameBoard == null || game.OpGameBoard == null) return;
        game.OnAllReady += OnAllReadyHandler;
        game.OnTimeLeftChanged += OnTimeLeftChangedHandler;
        game.OnGameFinishedUI += OnGameFinishedUIHandler;
        connectivity.ConnectivityChanged += OnConnectivityChanged;
    }
    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the UI grids for both the player and opponent game boards.
    /// Sets the grid cell sizes according to constants.
    /// </summary>
    public void InitializeGrid()
    {
        if (CurrentGame.GameBoard == null || CurrentGame.OpGameBoard == null
            || GameBoardGrid == null || OpGameBoardGrid == null) return;
        CurrentGame.GameBoard.InitializeGrid(GameBoardGrid);
        CurrentGame.OpGameBoard.InitializeGrid(OpGameBoardGrid);
    }

    /// <summary>
    /// Adds a listener for the "ready" event from the game.
    /// </summary>
    public void AddReadyListener()
    {
        CurrentGame.AddReadyListener();
    }

    /// <summary>
    /// Removes all registered game listeners to prevent memory leaks.
    /// </summary>
    public void RemoveGameListener()
    {
        CurrentGame.RemoveGameListener();
    }
    #endregion

    #region Private Methods

    /// <summary>
    /// Handles the game finished event for the UI.
    /// Updates visibility, result text, and result color depending on the winner.
    /// </summary>
    private void OnGameFinishedUIHandler(object? sender, EventArgs e)
    {
        bool isOpLost = sender is GameBoard gameBoard && gameBoard.IsOp;
        IsGameFinishedVisible = true;
        GameFinishedResultColor = isOpLost ? Colors.Green : Colors.Red;
        GameFinishedResultText = isOpLost ? Strings.YouWon : Strings.YouLost;
        OnPropertyChanged(nameof(IsGameFinishedVisible));
        OnPropertyChanged(nameof(GameFinishedResultColor));
        OnPropertyChanged(nameof(GameFinishedResultText));
    }

    /// <summary>
    /// Updates the UI timer and triggers the game start when the countdown ends.
    /// </summary>
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

    /// <summary>
    /// Handles the "all ready" event from the game.
    /// Hides the ready button, shows the timer, and prepares the game.
    /// </summary>
    private void OnAllReadyHandler(object? sender, EventArgs e)
    {
        IsReadyVisible = false;
        IsTimerVisible = true;

        OnPropertyChanged(nameof(IsReadyVisible));
        OnPropertyChanged(nameof(IsTimerVisible));

        CurrentGame.PrepareGame();
    }

    /// <summary>
    /// Navigates to the game lobby page.
    /// </summary>
    private void NavToGameLobby()
    {
        _ = Shell.Current.Navigation.PushAsync(new GameLobbyPage());
    }

    /// <summary>
    /// Marks the player as ready in the current game.
    /// </summary>
    private void Ready()
    {
        CurrentGame.Ready();
    }

    /// <summary>
    /// Moves the active shape one step to the right.
    /// </summary>
    private void MoveRightShape()
    {
        CurrentGame.MoveRightShape();
    }

    /// <summary>
    /// Moves the active shape one step to the left.
    /// </summary>
    private void MoveLeftShape()
    {
        CurrentGame.MoveLeftShape();
    }

    /// <summary>
    /// Moves the active shape one step down.
    /// </summary>
    private void MoveDownShape()
    {
        CurrentGame.MoveDownShape();
    }

    /// <summary>
    /// Moves the active shape steps down, as much as it can.
    /// </summary>
    private void SnapDownShape()
    {
        CurrentGame.SnapDownShape();
    }

    /// <summary>
    /// Rotates the active shape clockwise.
    /// </summary>
    private void RotateShape()
    {
        CurrentGame.RotateShape();
    }

    /// <summary>
    /// Handles connectivity state changes from the <see cref="Connectivity"/> model.
    /// Updates UI bindings and triggers game logic when the internet connection is lost.
    /// </summary>
    /// <param name="sender">The source of the connectivity change event.</param>
    /// <param name="e">Event arguments.</param>
    private void OnConnectivityChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(IsNoInternetVisible));
        CurrentGame.UpdateInternet(connectivity.IsConnected);
    }
    #endregion
}
