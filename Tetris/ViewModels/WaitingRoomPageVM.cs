using System.Windows.Input;
using Tetris.ModelsLogic;
using Tetris.Views;
using Tetris.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
            CurrentGame.GameBoard!.GameID = game.GameID;
            CurrentGame.OnPlayersChange += OnPlayersChange;
            CurrentGame.OnGameFull += OnGameFull;
        }

        private async void OnGameFull(object? sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new GamePage(CurrentGame));
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

        public void AddWaitingRoomListener()
        {
            CurrentGame.AddWaitingRoomListener();
        }

        public void RemoveWaitingRoomListener()
        {
            CurrentGame.RemoveWaitingRoomListener();
        }
    }
}
