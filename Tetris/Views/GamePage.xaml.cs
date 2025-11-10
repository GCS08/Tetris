using Tetris.ModelsLogic;
using Tetris.ViewModels;

namespace Tetris.Views;

public partial class GamePage : ContentPage
{
	public GamePage(Game game)
	{
		InitializeComponent();
        BindingContext = new GamePageVM(game);
    }
}