using Tetris.ViewModels;

namespace Tetris.Views;

public partial class NewGameConfigPage : ContentPage
{
	public NewGameConfigPage()
	{
		InitializeComponent();
        BindingContext = new NewGameConfigPageVM();
    }
}