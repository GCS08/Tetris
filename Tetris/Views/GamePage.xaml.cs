using Tetris.ModelsLogic;
using Tetris.ViewModels;

namespace Tetris.Views;

public partial class GamePage : ContentPage
{
    private readonly GamePageVM gpVM;
    public GamePage(Game game)
	{
		InitializeComponent();
        gpVM = new GamePageVM(game);
        BindingContext = gpVM;
        gpVM.GameBoardGrid = GameBoardYes;
    }
}