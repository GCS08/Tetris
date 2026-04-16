using System.Windows.Input;
using Tetris.Interfaces;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    /// <summary>
    /// ViewModel for the New Game Configuration page.
    /// Manages the creation of a new game, selection of color and privacy, and navigation.
    /// </summary>
    public partial class NewGameConfigPageVM : ObservableObject
    {
        #region Fields

        private readonly Game currentNewGame;
        private readonly JoinableGamesList gamesList;
        private readonly User User = IPlatformApplication.Current?.
            Services.GetService<IUser>() as User ?? new();
        #endregion

        #region ICommands
        public ICommand NavGameLobbyCommand => new Command(NavGameLobby);
        public ICommand CreateGameCommand => new Command(CreateGame);
        #endregion

        #region Properties
        public bool IsBusy { get; set; } = false;
        public bool IsCreateEnabled { get; set; } = true;

        public string SelectedColor
        {
            get => currentNewGame.CubeColor;
            set
            {
                if (currentNewGame.CubeColor != value)
                {
                    currentNewGame.CubeColor = value;
                    OnPropertyChanged(nameof(currentNewGame.CubeColor));
                }
            }
        }

        public string SelectedPrivacy
        {
            get => !currentNewGame.IsPublicGame ? Keys.PrivateKey : Keys.PublicKey;
            set
            {
                string PublicOrProtected = currentNewGame.IsPublicGame
                    ? Keys.PublicKey : Keys.PrivateKey;
                if (PublicOrProtected != value)
                {
                    currentNewGame.IsPublicGame = (value == Keys.PublicKey);
                    OnPropertyChanged(nameof(currentNewGame.IsPublicGame));
                }
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="NewGameConfigPageVM"/>.
        /// Sets up a new game instance and references the joinable games list.
        /// </summary>
        /// <param name="joinableGamesList">The list of joinable games to which the new game will be added.</param>
        public NewGameConfigPageVM(JoinableGamesList joinableGamesList)
        {
            currentNewGame = new(Keys.RedKey, User?.UserName ??
                Strings.UsernameUa, 1, 2, true, new(), string.Empty);
            gamesList = joinableGamesList;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a new game, adds it to the database, and navigates to the waiting room page.
        /// </summary>
        private void CreateGame()
        {
            if (User == null) return;
            UpdatePropertiesByBusy(true);
            gamesList.AddGameToDB(currentNewGame, User);
            UpdatePropertiesByBusy(false);
            Shell.Current.Navigation.PushAsync(
                new WaitingRoomPage(currentNewGame));
        }

        /// <summary>
        /// Updates the busy state and the create button enabled state.
        /// </summary>
        /// <param name="value">True if busy, false otherwise.</param>
        private void UpdatePropertiesByBusy(bool value)
        {
            IsBusy = value;
            OnPropertyChanged(nameof(IsBusy));
            IsCreateEnabled = !value;
            OnPropertyChanged(nameof(IsCreateEnabled));
        }

        /// <summary>
        /// Navigates back to the Game Lobby page.
        /// </summary>
        private void NavGameLobby()
        {
            Shell.Current.Navigation.PushAsync(new GameLobbyPage());
        }

        #endregion
    }
}