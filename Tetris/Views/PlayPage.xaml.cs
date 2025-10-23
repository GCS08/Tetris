using Tetris.ViewModels;

namespace Tetris.Views;

public partial class PlayPage : ContentPage
{
	public PlayPage()
	{
		InitializeComponent();
        BindingContext = new PlayPageVM();
    }
}