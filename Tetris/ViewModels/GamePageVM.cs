using System.Windows.Input;
using Tetris.Models;

namespace Tetris.ViewModels
{
    public class GamePageVM
    {
        public ICommand NavBackHomeCommand => new Command(NavHome);

        private async void NavHome(object obj)
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectMainPageRefresh);
        }
    }
}
