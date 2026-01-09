using CommunityToolkit.Mvvm.Messaging;
using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using System.Timers;
using Tetris.Models;
using Tetris.Views;

namespace Tetris.ModelsLogic
{
    public class Game : GameModel
    {
        public Game(string CubeColor, string CreatorName, int CurrentPlayersCount,
        int MaxPlayersCount, bool IsPublicGame, Shape shape, string GameID)
        {
            RegisterTimer();
            this.CubeColor = CubeColor;
            this.CreatorName = CreatorName;
            this.CurrentPlayersCount = CurrentPlayersCount;
            this.MaxPlayersCount = MaxPlayersCount;
            this.IsPublicGame = IsPublicGame;
            this.GameID = GameID;
            this.GameBoard = new(GameID, shape, false);
            this.OpGameBoard = new(GameID, shape.Duplicate(shape), true);
            OpFallTimer.Elapsed += ApplyOpMove;
            UsersInGame.Add((Application.Current as App)!.user);
        }
        protected override void RegisterTimer()
        {
            WeakReferenceMessenger.Default.Register<AppMessage<long>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
        }
        protected override void OnMessageReceived(long timeLeft)
        {
            TimeLeftMs = timeLeft;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnTimeLeftChanged?.Invoke(this, EventArgs.Empty);
            });
        }
        public override async Task OnPlayerLeaveWR()
        {
            if (CurrentPlayersCount <= 1)
            {
                ilr?.Remove();
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
        public override void AddWaitingRoomListener()
        {
            ilr = fbd.AddGameListener(GameID, OnChangeWaitingRoom!);
        }
        public override void RemoveWaitingRoomListener()
        {
            ilr?.Remove();
        }
        public override void AddGameListener()
        {
            ilr = fbd.AddGameListener(GameID, OnChangeGame!);
        }
        public override void RemoveGameListener()
        {
            ilr?.Remove();
        }
        public override void AddReadyListener()
        {
            ilr = fbd.AddGameListener(GameID, OnChangeReady!);
        }
        public override void RemoveReadyListener()
        {
            ilr?.Remove();
        }
        protected override void OnChangeReady(IDocumentSnapshot snapshot, Exception error)
        {
            bool allReady = true;
            for (int i = 0; i < MaxPlayersCount && allReady; i++)
            {
                if (!snapshot.Get<bool>(Keys.PlayerDetailsKey + i + 
                    TechnicalConsts.DotSign + Keys.IsPlayerReadyKey))
                    allReady = false;
            }
            if (allReady)
                OnAllReady?.Invoke(this, EventArgs.Empty);
        }
        protected override async void OnChangeGame(IDocumentSnapshot snapshot, Exception error)
        {
            bool found = false;
            for (desiredIndex = 0; desiredIndex < MaxPlayersCount && !found; desiredIndex++)
                if (snapshot.Get<bool>(Keys.PlayerDetailsKey + desiredIndex + 
                    TechnicalConsts.DotSign + Keys.IsShapeAtBottomKey))
                    found = true;

            if (GameBoard!.ShapesQueue!.IsEmpty() || snapshot.Get<int>(Keys.CurrentShapeMapKey + TechnicalConsts.DotSign 
                + Keys.CurrentShapeInGameIdKey) != GameBoard!.ShapesQueue!.GetTail().InGameId ||
                OpGameBoard!.ShapesQueue!.IsEmpty() || snapshot.Get<int>(Keys.CurrentShapeMapKey + TechnicalConsts.DotSign
                + Keys.CurrentShapeInGameIdKey) != OpGameBoard!.ShapesQueue!.GetTail().InGameId)// Shape has changed
            {
                Shape newShape = fbd.CreateShape(snapshot);
                Shape newShape2 = fbd.CreateShape(snapshot);
                if (GameBoard!.ShapesQueue!.IsEmpty() || snapshot.Get<int>(Keys.CurrentShapeMapKey + TechnicalConsts.DotSign
                + Keys.CurrentShapeInGameIdKey) != GameBoard!.ShapesQueue!.GetTail().InGameId)
                    GameBoard!.ShapesQueue.Insert(newShape);
                if (OpGameBoard!.ShapesQueue!.IsEmpty() || snapshot.Get<int>(Keys.CurrentShapeMapKey + TechnicalConsts.DotSign
                + Keys.CurrentShapeInGameIdKey) != OpGameBoard!.ShapesQueue!.GetTail().InGameId)
                    OpGameBoard!.ShapesQueue!.Insert(newShape2);
            }
            else if (found && snapshot.Get<string>(Keys.PlayerDetailsKey + (desiredIndex - 1) + TechnicalConsts.DotSign
                    + Keys.PlayerIdKey) != (Application.Current as App)!.user.UserID)// Op player has moved
            {
                Dictionary<int, string> playerMoveMap = snapshot.Get<Dictionary<int, string>>(
                    Keys.PlayerDetailsKey + (desiredIndex - 1) + TechnicalConsts.DotSign + Keys.PlayerMovesKey)!;

                currentMovingOp = snapshot.Get<string>(Keys.PlayerDetailsKey + (desiredIndex - 1) + TechnicalConsts.DotSign + Keys.PlayerIdKey)!;

                await fbd.ResetMoves(GameID, desiredIndex);

                for (int i = 0; i < playerMoveMap.Count; i++)
                    movesQueue.Insert(playerMoveMap[i]);

                System.Diagnostics.Debug.WriteLine(movesQueue.PrintQueue(out int counter) + "\nNodes in queue = " + counter);

                OpFallTimer.Start();
            }
        }
        protected override void ApplyOpMove(object? sender, ElapsedEventArgs e)
        {
            if (!movesQueue.IsEmpty() && GameBoard!.User!.UserID != currentMovingOp)
                // second check is unnecessary since it enters the second if in the on change game only if its not the player who finished a moved.
            {
                string move = movesQueue.Remove();
                switch (move)
                {
                    case Keys.RightKey:
                        MoveRightOpShape();
                        break;
                    case Keys.LeftKey:
                        MoveLeftOpShape();
                        break;
                    case Keys.DownKey:
                        MoveDownOpShape();
                        break;
                    case Keys.RotateKey:
                        RotateOpShape();
                        break;
                }
            }
            else
                OpFallTimer.Stop();
        }
        protected override void OnChangeWaitingRoom(IDocumentSnapshot snapshot, Exception error)
        {
            CurrentPlayersCount = snapshot.Get<int>(Keys.CurrentPlayersCountKey);
            fbd.GetPlayersFromDocument(GameID, OnCompleteChange!);
            if (IsFull)
            {
                fbd.SetGameIsFull(GameID);
                OnGameFull?.Invoke(this, null!);   
            }
        }
        protected override void OnCompleteChange(ObservableCollection<User> users)
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
        public override void PrepareGame()
        {
            WeakReferenceMessenger.Default.Send(
                new AppMessage<StartGameTimerSettings>(startGameTimerSettings));

            GameBoard!.User = (Application.Current as App)!.user;
            foreach (User user in UsersInGame)
                if (user != (Application.Current as App)!.user)
                    OpGameBoard!.User = user;

            RemoveReadyListener();
        }
        public override void StartGame()
        {
            AddGameListener();
            GameBoard!.StartGame();
            OpGameBoard!.StartGame();
        }
        public override async void MoveRightShape()
        {
            GameBoard!.MoveRightShape();
            await fbd.PlayerAction(GameID, (Application.Current as App)!.user.UserID, Keys.RightKey);
        }
        public override async void MoveLeftShape()
        {
            GameBoard!.MoveLeftShape();
            await fbd.PlayerAction(GameID, (Application.Current as App)!.user.UserID, Keys.LeftKey);
        }
        public override async void MoveDownShape()
        {
            bool isAtBottom = await GameBoard!.MoveDownShape();
            if (!isAtBottom)
                await fbd.PlayerAction(GameID, (Application.Current as App)!.user.UserID, Keys.DownKey);
        }
        public override async void RotateShape()
        {
            GameBoard!.RotateShape();
            await fbd.PlayerAction(GameID, (Application.Current as App)!.user.UserID, Keys.RotateKey);
        }
        public override void MoveRightOpShape()
        {
            OpGameBoard!.MoveRightShape();
        }
        public override void MoveLeftOpShape()
        {
            OpGameBoard!.MoveLeftShape();
        }
        public override async void MoveDownOpShape()
        {
            await OpGameBoard!.MoveDownShape();
        }
        public override void RotateOpShape()
        {
            OpGameBoard!.RotateShape();
        }
        public override void Ready()
        {
            fbd.SetPlayerReady(GameID, MaxPlayersCount, (Application.Current as App)!.user.UserID);
        }
    }
}
