using Tetris.ViewModels;

namespace Tetris.Views;

public partial class RemindersPage : ContentPage
{
	public RemindersPage()
	{
		InitializeComponent();
        BindingContext = new RemindersPageVM();
    }
}