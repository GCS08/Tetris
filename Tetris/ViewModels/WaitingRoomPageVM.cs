using System.Windows.Input;
using Tetris.ModelsLogic;
using Tetris.Models;

namespace Tetris.ViewModels
{
    public class WaitingRoomPageVM
    {
        public ICommand NavToGameLobbyCommand => new Command(NavToGameLobby);
        public WaitingRoomPageVM(JoinableGame game)
        {
        }
        private async void NavToGameLobby(object obj)
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectGameLobbyPage);
        }
    }
}
