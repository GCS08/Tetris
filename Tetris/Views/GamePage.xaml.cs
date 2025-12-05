using Tetris.ModelsLogic;
using Tetris.ViewModels;
using Tetris.Models;

namespace Tetris.Views;

public partial class GamePage : ContentPage
{
    private readonly GamePageVM gpVM;
    public GamePage(Game game)
	{
		InitializeComponent();
        gpVM = new GamePageVM(game)
        {
            GameBoardGrid = GameBoardGrid,
            OpGameBoardGrid = OpGameBoardGrid
        };
        BindingContext = gpVM;
        gpVM.InitializeGrid();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        gpVM.AddListener();

        // Give the UI time to actually render
        await Task.Delay(1000);

        // Now start the game
        gpVM.CurrentGame.StartGame();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        gpVM.RemoveListener();
    }
}