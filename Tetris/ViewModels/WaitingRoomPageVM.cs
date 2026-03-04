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
        #region ICommands
        public ICommand NavToGameLobbyCommand => new Command(NavToGameLobby);
        #endregion

        #region Properties
        public string PrivateJoinCode => !CurrentGame.IsPublicGame ? 
            Strings.CodeInterview + CurrentGame.PrivateJoinCode.ToString() : string.Empty;
        public ObservableCollection<User> PlayersInGame => CurrentGame.UsersInGame;
        private Game CurrentGame { get; set; }
        #endregion

        #region Constructors
        public WaitingRoomPageVM(Game? game)
        {
            if (game == null)
            {
                CurrentGame = null!; // intentional
                return;
            }
            CurrentGame = game;
            game.RegisterTimer();
            CurrentGame.OnPlayersChange += OnPlayersChange;
            CurrentGame.OnGameFull += OnGameFull;
            CurrentGame.OnCodeReady += OnCodeReady;
            if (!CurrentGame.IsPublicGame && CurrentGame.PrivateJoinCode == 0)
                CurrentGame.CreateCode();
        }
        #endregion

        #region Public Methods
        public void AddWaitingRoomListener()
        {
            CurrentGame.AddWaitingRoomListener();
        }

        public void RemoveWaitingRoomListener()
        {
            CurrentGame.RemoveWaitingRoomListener();
        }
        #endregion

        #region Private Methods
        private void OnCodeReady(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(PrivateJoinCode));
        }

        private void OnGameFull(object? sender, EventArgs e)
        {
            Shell.Current.Navigation.PushAsync(new GamePage(CurrentGame));
        }

        private void OnPlayersChange(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(PlayersInGame));
        }

        private async void NavToGameLobby()
        {
            await CurrentGame.OnPlayerLeaveWR();
            _ = Shell.Current.Navigation.PushAsync(new GameLobbyPage());
        }
        #endregion
    }
}
