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
            this.GameBoard = new(shape, ConstData.GameGridColumnWidth, ConstData.GameGridRowHeight);
            this.OpGameBoard = new(shape, ConstData.OpGameGridColumnWidth, ConstData.OpGameGridRowHeight);
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

        public void AddWaitingRoomListener()
        {
            ilr = fbd.AddWaitingRoomListener(GameID, OnChangeWaitingRoom!);
        }
        public void RemoveWaitingRoomListener()
        {
            ilr?.Remove();
        }

        public void AddGameListener()
        {
            ilr = fbd.AddGameListener(GameID, OnChangeGame!);
        }
        public void RemoveGameListener()
        {
            ilr?.Remove();
        }
        private async void OnChangeGame(IDocumentSnapshot snapshot, Exception error)
        {
            if (GameBoard!.ShapesQueue!.IsEmpty() || snapshot.Get<int>(Keys.CurrentShapeMapKey + "." + Keys.CurrentShapeInGameIdKey) != GameBoard!.ShapesQueue.GetTail().InGameId) //Shape has changed
            {
                GameBoard!.ShapesQueue.Insert(FbData.CreateShape(snapshot));
            }
            else if (snapshot.Get<string>(Keys.PlayerActionMapKey + "." + Keys.UserIDKey) != (Application.Current as App)!.user.UserID) //op Move has changed
            {
                switch (snapshot.Get<string>(Keys.PlayerActionMapKey + "." + Keys.PlayerActionKey))
                {
                    case Keys.LeftKey:
                        MoveLeftOpShape();
                        break;
                    case Keys.RightKey:
                        MoveRightOpShape();
                        break;
                    case Keys.DownKey:
                        MoveDownOpShape();
                        break;
                    case Keys.RotateKey:
                        RotateOpShape();
                        break;
                    default:
                        break;
                }
            }
        }
        private async void OnChangeWaitingRoom(IDocumentSnapshot snapshot, Exception error)
        {
            CurrentPlayersCount = snapshot.Get<int>(Keys.CurrentPlayersCountKey);
            fbd.GetPlayersFromDocument(GameID, OnCompleteChange!);
            if (IsFull)
            {
                fbd.SetGameIsFull(GameID);
                OnGameFull?.Invoke(this, null!);   
            }
        }
        private void OnCompleteChange(ObservableCollection<User> users)
        {
            UsersInGame.Clear();
            foreach (User user in users) { UsersInGame.Add(user); }
            OnPlayersChange!.Invoke(this, EventArgs.Empty);
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
            GameBoard!.StartGame();
            OpGameBoard!.StartGame();
        }
        public void MoveRightShape()
        {
            GameBoard!.MoveRightShape();
            fbd.PlayerAction(GameID, (Application.Current as App)!.user.UserID, Keys.RightKey);
        }

        public void MoveLeftShape()
        {
            GameBoard!.MoveLeftShape();
            fbd.PlayerAction(GameID, (Application.Current as App)!.user.UserID, Keys.LeftKey);
        }

        public void MoveDownShape()
        {
            GameBoard!.MoveDownShape();
            fbd.PlayerAction(GameID, (Application.Current as App)!.user.UserID, Keys.DownKey);
        }

        public void RotateShape()
        {
            GameBoard!.RotateShape();
            fbd.PlayerAction(GameID, (Application.Current as App)!.user.UserID, Keys.RotateKey);
        }
        public void MoveRightOpShape()
        {
            OpGameBoard!.MoveRightShape();
        }

        public void MoveLeftOpShape()
        {
            OpGameBoard!.MoveLeftShape();
        }

        public void MoveDownOpShape()
        {
            OpGameBoard!.MoveDownShape();
        }

        public void RotateOpShape()
        {
            OpGameBoard!.RotateShape();
        }
    }
}
