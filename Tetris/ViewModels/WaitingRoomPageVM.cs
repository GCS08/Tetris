using System.Windows.Input;
using Tetris.ModelsLogic;
using Tetris.Views;
using Tetris.Models;

namespace Tetris.ViewModels
{
    public class WaitingRoomPageVM(Game game)
    {
        Game CurrentGame { get; set; } = game;
        public ICommand NavToGameLobbyCommand => new Command(NavToGameLobby);
        private async void NavToGameLobby()
        {
            await CurrentGame.OnPlayerLeaveWR();
            await Shell.Current.Navigation.PushAsync(new GameLobbyPage());
        }
    }
}
