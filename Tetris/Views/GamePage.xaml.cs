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
        gpVM.AddGameListener();
        await Task.Delay(ConstData.SecondsTillGameStart * 1000);
        gpVM.CurrentGame.StartGame();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        gpVM.RemoveGameListener();
    }
}