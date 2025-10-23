using Tetris.ViewModels;

namespace Tetris.Views;

public partial class GamePage : ContentPage
{
	public GamePage()
	{
		InitializeComponent();
        BindingContext = new GamePageVM();
    }
}