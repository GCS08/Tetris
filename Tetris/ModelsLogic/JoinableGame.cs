using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class JoinableGame(string CubeColor, string CreatorName, int CurrentPlayersCount,
        int MaxPlayersCount, bool IsPublicGame, string GameID) :
        JoinableGameModel(CubeColor, CreatorName, CurrentPlayersCount, MaxPlayersCount,
            IsPublicGame, GameID)
    {
        public override async void NavToGame()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectMainPageRefresh);
        }
    }
}
