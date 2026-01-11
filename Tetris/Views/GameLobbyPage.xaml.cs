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
    protected async override void OnAppearing() // cannot be sync because of firestore method
    {
        base.OnAppearing();
        await glpVM.LoadGamesList();
        glpVM.AddGamesCollectionListener();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        glpVM.RemoveGamesCollectionListener();
    }
}