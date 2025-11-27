using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using Tetris.Models;
using Tetris.Views;

namespace Tetris.ModelsLogic
{
    public class Game : GameModel
    {
        public Game(string CubeColor, string CreatorName, int CurrentPlayersCount,
        int MaxPlayersCount, bool IsPublicGame, Shape shape, string GameID)
        {
            this.CubeColor = CubeColor;
            this.CreatorName = CreatorName;
            this.CurrentPlayersCount = CurrentPlayersCount;
            this.MaxPlayersCount = MaxPlayersCount;
            this.IsPublicGame = IsPublicGame;
            this.GameID = GameID;
            this.GameBoard = new(shape);
            this.GameBoard.ShapesQueue!.Insert(shape);
            UsersInGame.Add((Application.Current as App)!.user);
        }
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

        public void StartGame()
        {
            GameBoard.StartGame();
        }
    }
}
