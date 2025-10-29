using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tetris.Models;

namespace Tetris.ViewModels
{
    public class NewGameConfigPageVM
    {
        public ICommand NavBackHomeCommand => new Command(NavHome);
        private async void NavHome()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectMainPageRefresh);
        }
    }
}
