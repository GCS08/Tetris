using Firebase.Auth;
using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    internal class GameLobbyPageVM : ObservableObject, IQueryAttributable
    {
        public List<JoinableGame> Games { get; set; } = [];
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand NavToGameCommand => new Command(NavToGame);
        public ICommand NavToNewGameConfigCommand => new Command(NavToNewGameConfigGame);
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            JoinableGamesList gamesList = new();
            Games = await gamesList.GetJoinableGamesAsync();
            OnPropertyChanged(nameof(Games));
        }
        private async void NavHome()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectMainPageRefresh);
        }
        private async void NavToGame()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectGamePage);
        }
        private async void NavToNewGameConfigGame()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectNewGameConfigPage);
        }
    }
}
