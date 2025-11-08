using Tetris.ModelsLogic;
using Tetris.ViewModels;
namespace Tetris.Views;

public partial class WaitingRoomPage : ContentPage
{
	private readonly WaitingRoomPageVM wrpVM;
	public WaitingRoomPage(Game game)
	{
		InitializeComponent();
		wrpVM = new(game);
		BindingContext = wrpVM;
    }

	protected override void OnAppearing()
	{
		base.OnAppearing();
		wrpVM.AddGameListener();
    }
    protected override void OnDisappearing()
	{
		base.OnDisappearing();
		wrpVM.RemoveGameListener();
    }
}