using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    internal class NewGameConfigPageVM : ObservableObject
    {
        public ICommand NavGameLobbyCommand => new Command(NavGameLobby);
        public ICommand CreateGameCommand { get; }
        private readonly JoinableGame currentNewGame;
        public bool IsBusy { get; set; } = false;
        public NewGameConfigPageVM()
        {
            CreateGameCommand = new Command(async () => await CreateGame());
            currentNewGame = new("Red", Preferences.Get(Keys.UserNameKey, "Anonymous"), 1, 2, true, string.Empty);
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
            IsBusy = true;
            OnPropertyChanged(nameof(IsBusy));
            await currentNewGame.AddGameToDB();
            IsBusy = false;
            OnPropertyChanged(nameof(IsBusy));
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectGamePage);
        }
        private async void NavGameLobby()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectGameLobbyPageRefresh);
        }
    }
}
