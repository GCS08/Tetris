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
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await glpVM.LoadGamesList();
        glpVM.AddSnapshotListener();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        glpVM.RemoveSnapshotListener();
    }
}