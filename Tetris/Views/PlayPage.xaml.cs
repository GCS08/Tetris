using Tetris.ViewModels;

namespace Tetris.Views;

public partial class PlayPage : ContentPage
{
	public PlayPage()
	{
		InitializeComponent();
        BindingContext = new PlayPageVM();
    }
	protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PlayPageVM vm)
            await vm.InitializeAsync();
    }
}