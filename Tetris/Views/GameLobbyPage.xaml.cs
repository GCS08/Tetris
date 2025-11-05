using Tetris.ViewModels;

namespace Tetris.Views;

public partial class GameLobbyPage : ContentPage
{
    private readonly GameLobbyPageVM glpVM = new();
	public GameLobbyPage()
	{
		InitializeComponent();
        BindingContext = glpVM;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        glpVM.AddSnapshotListener();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        glpVM.RemoveSnapshotListener();
    }
}