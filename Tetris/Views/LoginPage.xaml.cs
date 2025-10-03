using Tetris.ViewModels;

namespace Tetris.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
        InitializeComponent();
		BindingContext = new LoginPageVM();
	}
}