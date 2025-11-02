using Tetris.ViewModels;

namespace Tetris.Views;

public partial class GameLobbyPage : ContentPage
{
	public GameLobbyPage()
	{
		InitializeComponent();
        BindingContext = new GameLobbyPageVM();
    }
}