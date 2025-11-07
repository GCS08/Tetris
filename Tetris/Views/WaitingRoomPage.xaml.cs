using Tetris.ModelsLogic;
using Tetris.ViewModels;
namespace Tetris.Views;

public partial class WaitingRoomPage : ContentPage
{
	public WaitingRoomPage(Game game)
	{
		InitializeComponent();
		BindingContext = new WaitingRoomPageVM(game);
    }
}