using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class JoinableGame(string CubeColor, string CreatorName, int CurrentPlayersCount, int MaxPlayersCount, string GameID) : JoinableGameModel(CubeColor, CreatorName, CurrentPlayersCount, MaxPlayersCount, GameID)
    {
        public override async void NavToGame()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectMainPageRefresh);
        }
    }
}
