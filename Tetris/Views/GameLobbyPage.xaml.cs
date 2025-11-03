using Tetris.ViewModels;

namespace Tetris.Views;

public partial class GameLobbyPage : ContentPage
{
	public GameLobbyPage()
	{
		InitializeComponent();
        BindingContext = new GameLobbyPageVM();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
}