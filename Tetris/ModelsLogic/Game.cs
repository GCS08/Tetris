using Plugin.CloudFirestore;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class Game(string CubeColor, string CreatorName, int CurrentPlayersCount,
        int MaxPlayersCount, bool IsPublicGame, string GameID) :
        GameModel(CubeColor, CreatorName, CurrentPlayersCount, MaxPlayersCount,
            IsPublicGame, GameID)
    {
        public async Task OnPlayerLeaveWR()
        {
            if (CurrentPlayersCount <= 1)
                await fbd.DeleteGameFromDB(GameID);
            else
            {
                await fbd.OnPlayerLeaveWR(GameID);
                CurrentPlayersCount -= 1;
                UsersInGame.Remove(User);
            }            
        }

        public void AddGameListener()
        {
            ilr = fbd.AddGameListener(GameID, OnChange!);
        }
        private void OnChange(IDocumentSnapshot snapshot, Exception error)
        {
            if (error != null)
            {
                // handle the error
                return;
            }
            Game updatedGame = snapshot.ConvertTo<Game>()!;
            CurrentPlayersCount = updatedGame.CurrentPlayersCount;
            UsersInGame = updatedGame.UsersInGame;
        }
        public void RemoveGameListener()
        {
            throw new NotImplementedException();
        }
    }
}
