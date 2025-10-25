using System.Windows.Input;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    internal class PlayPageVM
    {
        public List<JoinableGame> Games;
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand NavToGameCommand => new Command(NavToGame);
        public ICommand NavToNewGameConfigCommand => new Command(NavToNewGameConfigGame);
        public PlayPageVM()
        {
            JoinableGamesList gamesList = new();
            Games = gamesList.games;
        }
        private async void NavHome()
        {
            await Shell.Current.GoToAsync("///MainPage?refresh=true");
        }
        private async void NavToGame()
        {
            await Shell.Current.GoToAsync("///GamePage");
        }
        private async void NavToNewGameConfigGame()
        {
            await Shell.Current.GoToAsync("///NewGameConfigPage");
        }
    }
}
