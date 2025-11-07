using Tetris.ModelsLogic;
using Tetris.ViewModels;

namespace Tetris.Views;

public partial class NewGameConfigPage : ContentPage
{
	public NewGameConfigPage(JoinableGamesList list)
	{
		InitializeComponent();
        BindingContext = new NewGameConfigPageVM(list);
    }
}