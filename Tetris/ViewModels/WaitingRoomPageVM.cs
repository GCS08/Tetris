using System.Windows.Input;
using Tetris.ModelsLogic;
using Tetris.Views;
using Tetris.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Tetris.ViewModels
{
    /// <summary>
    /// ViewModel for the Waiting Room page.
    /// Handles players joining, private game code display, and navigation to the game.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of <see cref="WaitingRoomPageVM"/> for the specified game.
        /// Registers event handlers for game updates and sets up a private code if needed.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        public WaitingRoomPageVM(Game game)
        {
            CurrentGame = game;
            CurrentGame.OnPlayersChange += OnPlayersChange;
            CurrentGame.OnGameFull += OnGameFull;
            CurrentGame.OnCodeReady += OnCodeReady;

            if (!CurrentGame.IsPublicGame && CurrentGame.PrivateJoinCode == 0)
                CurrentGame.CreateCode();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers listeners needed for the waiting room updates.
        /// </summary>
        public void AddGameListener()
        {
            CurrentGame.AddGameListener();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the private join code when it is ready.
        /// </summary>
        private void OnCodeReady(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(PrivateJoinCode));
        }

        /// <summary>
        /// Navigates to the game page when the game becomes full.
        /// </summary>
        private void OnGameFull(object? sender, EventArgs e)
        {
            Shell.Current.Navigation.PushAsync(new GamePage(CurrentGame));
        }

        /// <summary>
        /// Updates the observable collection of players when a change occurs.
        /// </summary>
        private void OnPlayersChange(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(PlayersInGame));
        }

        /// <summary>
        /// Handles navigation back to the game lobby, leaving the waiting room first.
        /// </summary>
        private void NavToGameLobby()
        {
            CurrentGame.OnPlayerLeaveWR();
            _ = Shell.Current.Navigation.PushAsync(new GameLobbyPage());
        }

        #endregion
    }
}