using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Plugin.CloudFirestore;
using Tetris.Models;
using Tetris.Views;

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
        private void OnChange(IDocumentSnapshot snapshot, Exception error)
        {
            fbd.GetPlayersFromDocument(GameID, OnCompleteChange!);
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
