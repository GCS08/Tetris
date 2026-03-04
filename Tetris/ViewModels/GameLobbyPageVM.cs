using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels
{
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
        public GameLobbyPageVM()
        {
            EnterPrivateGameCommand = new Command(EnterPrivateGame);
        }
        #endregion

        #region Public Methods
        public void AddGamesCollectionListener()
        {
            if (JoinableGamesList == null) return;
            JoinableGamesList.AddGamesCollectionListener();
        }
     
        public void RemoveGamesCollectionListener()
        {
            if (JoinableGamesList == null) return;
            JoinableGamesList.RemoveGamesCollectionListener();
        }
     
        public async Task LoadGamesList() 
        {
            if (JoinableGamesList == null) return;
            JoinableGamesList = await JoinableGamesList.CreateAsync();
            JoinableGamesList.OnGamesChanged += OnGamesChanged;
            Games = JoinableGamesList.gamesObsCollection;
            OnPropertyChanged(nameof(Games));
        }
        #endregion

        #region Private Methods
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
      
        private void OnGamesChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Games));
        }
      
        private void NavHome()
        {
            Shell.Current.Navigation.PushAsync(new MainPage());
        }
    
        private void NavToNewGameConfigGame()
        {
            if (JoinableGamesList == null) return;
            Shell.Current.Navigation.PushAsync(new NewGameConfigPage(JoinableGamesList));
        }
        #endregion
    }
}
