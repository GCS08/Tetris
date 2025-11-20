using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Plugin.CloudFirestore;
using Tetris.Models;
using Tetris.Views;

namespace Tetris.ModelsLogic
{
    public class Game(string CubeColor, string CreatorName, int CurrentPlayersCount,
        int MaxPlayersCount, bool IsPublicGame, int FirstShapeID, string GameID) :
        GameModel(CubeColor, CreatorName, CurrentPlayersCount, MaxPlayersCount,
            IsPublicGame, FirstShapeID, GameID)
    {
        public async Task OnPlayerLeaveWR()
        {
            if (CurrentPlayersCount <= 1)
            {
                ilr?.Remove();
                ilr = null;
                await fbd.DeleteGameFromDB(GameID);
            }
            else
            {
                await fbd.OnPlayerLeaveWR(GameID,
                    (Application.Current as App)!.user.UserID);
                CurrentPlayersCount--;
                UsersInGame.Remove((Application.Current as App)!.user);
            }            
        }

        public void AddGameListener()
        {
            ilr = fbd.AddGameListener(GameID, OnChange!);
        }
        private async void OnChange(IDocumentSnapshot snapshot, Exception error)
        {
            CurrentPlayersCount = await fbd.GetCurrentPlayersCount(GameID);
            fbd.GetPlayersFromDocument(GameID, OnCompleteChange!);
            if (IsFull)
                OnGameFull?.Invoke(this, null!);
        }
        private void OnCompleteChange(ObservableCollection<User> users)
        {
            UsersInGame.Clear();
            foreach (User user in users) { UsersInGame.Add(user); }
            OnPlayersChange!.Invoke(this, EventArgs.Empty);
        }
        public void RemoveGameListener()
        {
            ilr?.Remove();
            ilr = null;
        }

        public override async void NavToWR()
        {
            await fbd.OnPlayerJoinWR(GameID,
                (Application.Current as App)!.user.UserID);
            CurrentPlayersCount++;
            UsersInGame.Add((Application.Current as App)!.user);
            await Shell.Current.Navigation.PushAsync(new WaitingRoomPage(this));
        }
    }
}
