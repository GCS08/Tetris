using Firebase.Auth;
using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    internal class GameLobbyPageVM : ObservableObject
    {
        private readonly JoinableGamesList joinableGamesList = new();
        public List<JoinableGame> Games => joinableGamesList.list!;
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand NavToGameCommand => new Command(NavToGame);
        public ICommand NavToNewGameConfigCommand => new Command(NavToNewGameConfigGame);
        public void AddSnapshotListener()
        {
            joinableGamesList.AddSnapshotListener();
        }
        public void RemoveSnapshotListener()
        {
            joinableGamesList.RemoveSnapshotListener();
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
