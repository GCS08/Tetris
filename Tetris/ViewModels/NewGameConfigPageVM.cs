using System.Windows.Input;
using Tetris.Models;
using Tetris.Views;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    public partial class NewGameConfigPageVM : ObservableObject
    {
        public ICommand NavGameLobbyCommand => new Command(NavGameLobby);
        public ICommand CreateGameCommand { get; }
        private readonly Game currentNewGame;
        private readonly JoinableGamesList gamesList;
        public bool IsBusy { get; set; } = false;
        public bool IsCreateEnabled { get; set; } = true;
        private readonly User user;
        public NewGameConfigPageVM(JoinableGamesList joinableGamesList)
        {
            App? app = Application.Current as App;
            user = app!.user;
            CreateGameCommand = new Command(async () => await CreateGame());
            currentNewGame = new("Red", Preferences.Get(Keys.UserNameKey, "Anonymous"), 1, 2, true, 0, string.Empty);
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
            get => !currentNewGame.IsPublicGame ? "Private" : "Public";
            set
            {
                string PublicOrProtected = currentNewGame.IsPublicGame ? "Public" : "Private";
                if (PublicOrProtected != value)
                {
                    currentNewGame.IsPublicGame = (value == "Public");
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
        private async Task CreateGame()
        {
            UpdatePropertiesByBusy(true);
            await gamesList.AddGameToDB(currentNewGame,
                (Application.Current as App)!.user);
            UpdatePropertiesByBusy(false);
            await Shell.Current.Navigation.PushAsync(
                new WaitingRoomPage(currentNewGame));
        }
        private void UpdatePropertiesByBusy(bool value)
        {
            IsBusy = value;
            OnPropertyChanged(nameof(IsBusy));
            IsCreateEnabled = !value;
            OnPropertyChanged(nameof(IsCreateEnabled));
        }
        private async void NavGameLobby()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectGameLobbyPage);
        }
    }
}
