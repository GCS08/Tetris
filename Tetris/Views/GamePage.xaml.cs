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
        gpVM = new GamePageVM(game, GameBoardGrid, OpGameBoardGrid);
        BindingContext = gpVM;
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        gpVM.RemoveGameListener();
    }
}