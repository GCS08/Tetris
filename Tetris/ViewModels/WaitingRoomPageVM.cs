using System.Windows.Input;
using Tetris.ModelsLogic;
using Tetris.Views;
using Tetris.Models;
using System.Collections.ObjectModel;

namespace Tetris.ViewModels
{
    public partial class WaitingRoomPageVM : ObservableObject
    {
        private Game CurrentGame { get; set; }
        public ICommand NavToGameLobbyCommand => new Command(NavToGameLobby);
        public ObservableCollection<User> PlayersInGame => CurrentGame.UsersInGame;
        public WaitingRoomPageVM(Game game)
        {
            this.CurrentGame = game;
            CurrentGame.OnPlayersChange += OnPlayersChange;
        }

        private void OnPlayersChange(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(PlayersInGame));
        }

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
