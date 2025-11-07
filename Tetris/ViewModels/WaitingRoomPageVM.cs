using System.Windows.Input;
using Tetris.ModelsLogic;
using Tetris.Views;
using Tetris.Models;
using System.Collections.ObjectModel;

namespace Tetris.ViewModels
{
    public class WaitingRoomPageVM(Game game)
    {
        Game CurrentGame { get; set; } = game;
        public ICommand NavToGameLobbyCommand => new Command(NavToGameLobby);
        public ObservableCollection<User> UsersInGame => CurrentGame.UsersInGame;
        private async void NavToGameLobby()
        {
            await CurrentGame.OnPlayerLeaveWR();
            await Shell.Current.Navigation.PushAsync(new GameLobbyPage());
        }

        public void AddGameListener()
        {
            CurrentGame.AddGameListener();
        }

        public void RemoveGameListener()
        {
            CurrentGame.RemoveGameListener();
        }
    }
}
