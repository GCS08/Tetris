using System.Windows.Input;
using Tetris.Models;
using Tetris.Views;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    public partial class NewGameConfigPageVM : ObservableObject
    {
        public ICommand NavGameLobbyCommand => new Command(NavGameLobby);
        public ICommand CreateGameCommand => new Command(CreateGame);
        private readonly Game currentNewGame;
        private readonly JoinableGamesList gamesList;
        public bool IsBusy { get; set; } = false;
        public bool IsCreateEnabled { get; set; } = true;
        private readonly User user;
        public NewGameConfigPageVM(JoinableGamesList joinableGamesList)
        {
            user = (Application.Current as App)!.AppUser;
            currentNewGame = new(Keys.RedKey, user.UserName, 1, 2, true, new(0), string.Empty);
            gamesList = joinableGamesList;
        }
        public string SelectedColor
        {
            get => currentNewGame.CubeColor;
            set
            {
                if (currentNewGame.CubeColor != value)
                {
                    currentNewGame.CubeColor = value;
                    OnPropertyChanged(nameof(currentNewGame.CubeColor));
                    // You can react to selection change here if you want
                }
            }
        }
        public string SelectedPrivacy
        {
            get => !currentNewGame.IsPublicGame ? Keys.PrivateKey : Keys.PublicKey;
            set
            {
                string PublicOrProtected = currentNewGame.IsPublicGame ? Keys.PublicKey : Keys.PrivateKey;
                if (PublicOrProtected != value)
                {
                    currentNewGame.IsPublicGame = (value == Keys.PublicKey);
                    OnPropertyChanged(nameof(currentNewGame.IsPublicGame));
                    // You can react to selection change here if you want
                }
            }
        }
        public string SelectedMaxPlayers
        {
            get => currentNewGame.MaxPlayersCount.ToString();
            set
            {
                if (currentNewGame.MaxPlayersCount.ToString() != value)
                {
                    currentNewGame.MaxPlayersCount = int.Parse(value);
                    OnPropertyChanged(nameof(currentNewGame.MaxPlayersCount));
                    // You can react to selection change here if you want
                }
            }
        }
        private void CreateGame()
        {
            UpdatePropertiesByBusy(true);
            gamesList.AddGameToDB(currentNewGame,
                (Application.Current as App)!.AppUser);
            UpdatePropertiesByBusy(false);
            Shell.Current.Navigation.PushAsync(
                new WaitingRoomPage(currentNewGame));
        }
        private void UpdatePropertiesByBusy(bool value)
        {
            IsBusy = value;
            OnPropertyChanged(nameof(IsBusy));
            IsCreateEnabled = !value;
            OnPropertyChanged(nameof(IsCreateEnabled));
        }
        private void NavGameLobby()
        {
            Shell.Current.Navigation.PushAsync(new GameLobbyPage());
        }
    }
}
