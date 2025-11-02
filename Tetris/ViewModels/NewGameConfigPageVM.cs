using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    internal class NewGameConfigPageVM : ObservableObject
    {
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand CreateGameCommand => new Command(CreateGame);
        private readonly App? app;
        private User user;
        private JoinableGame currentNewGame;
        public NewGameConfigPageVM()
        {
            app = Application.Current as App;
            user = app!.user;

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
        private void CreateGame()
        {

        }
        private async void NavHome()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectMainPageRefresh);
        }
    }
}
