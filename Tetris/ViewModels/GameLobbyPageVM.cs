using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    /// <summary>
    /// ViewModel for the Game Lobby page.
    /// Handles the binding of the list of joinable games, user-entered private codes, 
    /// and interactions for navigating to other pages or entering a private game.
    /// </summary>
    public partial class GameLobbyPageVM : ObservableObject
    {
        #region Fields
        private int enteredCode;
        #endregion

        #region ICommands
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand NavToGameCommand => new Command(NavHome);
        public ICommand NavToNewGameConfigCommand => new Command(NavToNewGameConfigGame);
        public ICommand EnterPrivateGameCommand { get; private set; }
        #endregion

        #region Properties
        private JoinableGamesList? JoinableGamesList { get; set; } = new();
        public ObservableCollection<Game>? Games { get; private set; }
        public int EnteredCode
        {
            get => enteredCode;
            set
            {
                if (enteredCode != value)
                    enteredCode = value;
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="GameLobbyPageVM"/>.
        /// Sets up the command for entering a private game.
        /// </summary>
        public GameLobbyPageVM()
        {
            EnterPrivateGameCommand = new Command(EnterPrivateGame);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a listener to the joinable games collection so that the ViewModel 
        /// can respond to updates in real time.
        /// </summary>
        public void AddGamesCollectionListener()
        {
            if (JoinableGamesList == null) return;
            JoinableGamesList.AddGamesCollectionListener();
        }

        /// <summary>
        /// Removes the listener from the joinable games collection.
        /// Should be called when the ViewModel is no longer active to avoid memory leaks.
        /// </summary>
        public void RemoveGamesCollectionListener()
        {
            if (JoinableGamesList == null) return;
            JoinableGamesList.RemoveGamesCollectionListener();
        }

        /// <summary>
        /// Asynchronously loads the current list of joinable games from the database
        /// and binds it to the <see cref="Games"/> collection for UI display.
        /// </summary>
        public async Task LoadGamesList()
        {
            if (JoinableGamesList == null) return;
            JoinableGamesList = await JoinableGamesList.CreateAsync();
            JoinableGamesList.OnGamesChanged += OnGamesChanged;
            Games = JoinableGamesList.GamesObsCollection;
            OnPropertyChanged(nameof(Games));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Attempts to join a private game using the user-entered code.
        /// Navigates to the Waiting Room page if successful; otherwise shows a toast.
        /// </summary>
        private async void EnterPrivateGame()
        {
            Game currentGame = new();
            currentGame = await currentGame.GetGameByCode(EnteredCode);
            if (currentGame != null)
            {
                currentGame.PrivateJoinCode = EnteredCode;
                currentGame.NavToWR();
            }
            else
            {
                _ = Toast.Make(Strings.FalseCode,
                    CommunityToolkit.Maui.Core.ToastDuration.Long,
                    ConstData.ToastFontSize - 4).Show();
                EnteredCode = 0;
                OnPropertyChanged(nameof(EnteredCode));
            }
        }

        /// <summary>
        /// Called when the list of joinable games changes.
        /// Notifies the UI that the <see cref="Games"/> property has changed.
        /// </summary>
        private void OnGamesChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Games));
        }

        /// <summary>
        /// Navigates back to the main page.
        /// </summary>
        private void NavHome()
        {
            Shell.Current.Navigation.PushAsync(new MainPage());
        }

        /// <summary>
        /// Navigates to the page for configuring a new game.
        /// Passes the current <see cref="JoinableGamesList"/> for context.
        /// </summary>
        private void NavToNewGameConfigGame()
        {
            if (JoinableGamesList == null) return;
            Shell.Current.Navigation.PushAsync(new NewGameConfigPage(JoinableGamesList));
        }

        #endregion
    }
}
