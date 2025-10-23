using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Tetris.ViewModels
{
    internal class PlayPageVM
    {
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand NavToGameCommand => new Command(NavToGame);
        public ICommand NavToNewGameConfigCommand => new Command(NavToNewGameConfigGame);
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
