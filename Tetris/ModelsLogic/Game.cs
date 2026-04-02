using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.CloudFirestore;
using Tetris.Models;
using Tetris.Views;

namespace Tetris.ModelsLogic
{
    /// <summary>
    /// Represents a Tetris game instance, managing both the player's and opponent's boards,
    /// moves, game state, and integration with Firestore for multiplayer updates.
    /// </summary>
    public class Game : GameModel
    {
        #region Constructors

        /// <summary>
        /// Initializes an empty game instance. Used for deserialization or delayed setup.
        /// </summary> 
        public Game() { }

        /// <summary>
        /// Initializes a game with the specified settings, creating both player and opponent boards.
        /// </summary>
        /// <param name="CubeColor">The color used for cubes on the board.</param>
        /// <param name="CreatorName">The name of the game creator.</param>
        /// <param name="CurrentPlayersCount">The current number of players in the game.</param>
        /// <param name="MaxPlayersCount">The maximum allowed players in the game.</param>
        /// <param name="IsPublicGame">Indicates whether the game is public or private.</param>
        /// <param name="firstShape">The initial shape to display on the boards.</param>
        /// <param name="secondShape">The second shape to display on the boards.</param>
        /// <param name="GameID">Unique identifier for the game.</param>
        public Game(string CubeColor, string CreatorName, int CurrentPlayersCount,
        int MaxPlayersCount, bool IsPublicGame, Shape firstShape, Shape secondShape, string GameID)
        {
            this.CubeColor = CubeColor;
            this.CreatorName = CreatorName;
            this.CurrentPlayersCount = CurrentPlayersCount;
            this.MaxPlayersCount = MaxPlayersCount;
            this.IsPublicGame = IsPublicGame;
            this.GameID = GameID;
            this.GameBoard = new(GameID, firstShape, secondShape, false);
            this.OpGameBoard = new(GameID, firstShape.Duplicate(), secondShape.Duplicate(), true);
            OpGameBoard.OnGameFinishedLogic += OnGameFinishedLogicHandler;
            GameBoard.OnGameFinishedLogic += OnGameFinishedLogicHandler;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves a game instance from Firestore by a private join code.
        /// </summary>
        /// <param name="code">The game's private join code.</param>
        /// <returns>The <see cref="Game"/> object if found; otherwise, null.</returns>
        public override async Task<Game> GetGameByCode(int code)
        {
            Game? result = await fbd.GetGameByCode(code);
            return result!;
        }

        /// <summary>
        /// Registers the timer to listen for countdown updates via <see cref="WeakReferenceMessenger"/>.
        /// </summary>
        public override void RegisterTimer()
        {
            WeakReferenceMessenger.Default.Register<AppMessage<long>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
        }

        /// <summary>
        /// Handles a player leaving the waiting room or the game,
        /// updating Firestore and removing the player locally.
        /// </summary>
        public override async Task OnPlayerLeaveWR() 
        {
            if (CurrentPlayersCount <= 1)
            {
                ilr?.Remove();
                fbd.DeleteGameFromDB(GameID);
            }
            else
            {
                await fbd.OnPlayerLeaveWR(GameID, User.UserID);
                CurrentPlayersCount--;
                UsersInGame.Remove(User);
            }            
        }

        /// <summary>
        /// Subscribes to Firestore updates for the waiting room.
        /// </summary>
        public override void AddWaitingRoomListener()
        {
            ilr = fbd.AddGameListener(GameID, OnChangeWaitingRoom);
        }

        /// <summary>
        /// Removes the waiting room listener from Firestore updates.
        /// </summary>
        public override void RemoveWaitingRoomListener()
        {
            ilr?.Remove();
        }

        /// <summary>
        /// Subscribes to Firestore updates for in-game changes.
        /// </summary>
        public override void AddGameListener()
        {
            ilr = fbd.AddGameListener(GameID, OnChangeGame);
        }

        /// <summary>
        /// Removes the in-game listener from Firestore updates.
        /// </summary>
        public override void RemoveGameListener()
        {
            ilr?.Remove();
        }

        /// <summary>
        /// Subscribes to Firestore updates for ready-state changes.
        /// </summary>
        public override void AddReadyListener()
        {
            ilr = fbd.AddGameListener(GameID, OnChangeReady);
        }

        /// <summary>
        /// Removes the ready-state listener from Firestore updates.
        /// </summary>
        public override void RemoveReadyListener()
        {
            ilr?.Remove();
        }

        /// <summary>
        /// Navigates the user to the waiting room and updates Firestore accordingly.
        /// </summary>
        public override async void NavToWR()
        {
            if (User == null) return;

            await fbd.OnPlayerJoinWR(GameID, User.UserID);
            CurrentPlayersCount++;

            _ = Shell.Current.Navigation.PushAsync(new WaitingRoomPage(this));
        }

        /// <summary>
        /// Prepares the game by sending start timer settings and clearing ready listeners.
        /// </summary>
        public override void PrepareGame()
        {
            WeakReferenceMessenger.Default.Send(
                new AppMessage<StartGameTimerSettings>(startGameTimerSettings));

            RemoveReadyListener();
        }

        /// <summary>
        /// Starts the game, initializing the opponent timer and enabling player moves.
        /// </summary>
        public override void StartGame()
        {
            if (GameBoard == null || OpGameBoard == null || IsGameStarted) return;

            OpFallTimer = Application.Current!.Dispatcher.CreateTimer();
            OpFallTimer.Interval =
                TimeSpan.FromSeconds(ConstData.OpShapeFallIntervalS);
            OpFallTimer.Tick += ApplyOpMove;

            AddGameListener();
            IsGameStarted = true;
            GameBoard.StartGame();
            OpGameBoard.EnableMoves = true;
        }

        /// <summary>
        /// Moves the player's current shape to the right, if possible.
        /// </summary>
        public override void MoveRightShape() 
        {
            if (GameBoard == null) return;
            GameBoard.MoveRightShape();
        }

        /// <summary>
        /// Moves the player's current shape to the left, if possible.
        /// </summary>
        public override void MoveLeftShape() 
        {
            if (GameBoard == null) return;
            GameBoard.MoveLeftShape();
        }

        /// <summary>
        /// Moves the player's current shape down, if possible.
        /// </summary>
        public override void MoveDownShape() 
        {
            if (GameBoard == null) return;
            GameBoard.MoveDownShape();
        }

        /// <summary>
        /// Moves the player's current shape low as possible.
        /// </summary>
        public override async void SnapDownShape() 
        {
            if (GameBoard == null) return;
            await GameBoard.SnapDownShape();
        }

        /// <summary>
        /// Rotates the player's current shape, applying wall kicks if necessary.
        /// </summary>
        public override void RotateShape() 
        {
            if (GameBoard == null) return;
            GameBoard.RotateShape();
        }

        /// <summary>
        /// Sets the player as ready in Firestore.
        /// </summary>
        public override void Ready()
        {
            if (User == null) return;
            fbd.SetPlayerReady(GameID, MaxPlayersCount, User.UserID);
        }

        /// <summary>
        /// Generates a private join code for this game and updates Firestore.
        /// </summary>
        public override async void CreateCode()
        {
            Random rnd = new();
            int code = 0;
            bool success = false;
            while (!success)
            {
                code = rnd.Next(100000, 1000000);
                success = await fbd.SetPrivateJoinCode(GameID, code);
            }
            PrivateJoinCode = code;
            OnCodeReady?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Protected Methods

        /// <summary>
        /// Handles logic when the game finishes, updating stats, stopping timers, and triggering UI events.
        /// </summary>
        /// <param name="sender">
        /// The source of the event. Usually the <see cref="GameBoard"/> or OpGameBoard/> 
        /// that triggered the game finished event.
        /// </param>
        /// <param name="e">Event arguments for the game finished event. Usually <see cref="EventArgs.Empty"/></param>
        protected override void OnGameFinishedLogicHandler(object? sender, EventArgs e)
        {
            if (GameBoard == null || OpGameBoard == null || GameBoard.User == null || 
                OpGameBoard.User == null || GameBoard.FallTimer == null || OpFallTimer == null) return;
            if (!IsStatsUpdatedOnceOnGameFinished)
            {
                GameBoard.User.HighestScore = GameBoard.User.HighestScore < GameBoard.Score
                    ? GameBoard.Score : GameBoard.User.HighestScore;
                GameBoard.User.GamesPlayed++;
                fbd.UpdateUserPostGame(GameBoard.User);
                IsStatsUpdatedOnceOnGameFinished = true;
            }
            UnregisterTimer();
            GameBoard.FallTimer.Stop();
            OpFallTimer.Stop();
            GameBoard.EnableMoves = false;
            OpGameBoard.EnableMoves = false;
            OnGameFinishedUI?.Invoke(sender as GameBoard, EventArgs.Empty);
        }

        /// <summary>
        /// Unregisters the timer from <see cref="WeakReferenceMessenger"/>.
        /// </summary>
        protected override void UnregisterTimer()
        {
            WeakReferenceMessenger.Default.Unregister<AppMessage<long>>(this);
        }

        /// <summary>
        /// Updates the remaining game time and triggers the OnTimeLeftChanged event.
        /// </summary>
        /// <param name="timeLeft">The remaining time in milliseconds.</param>
        protected override void OnMessageReceived(long timeLeft)
        {
            TimeLeftMs = timeLeft;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnTimeLeftChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// Called when a snapshot of the "ready" state changes in the database. 
        /// Checks if all players are ready and raises OnAllReady if so.
        /// </summary>
        /// <param name="snapshot">
        /// The Firestore document snapshot containing current ready states of all players.
        /// May be null if an error occurred or document is missing.
        /// </param>
        /// <param name="error">
        /// Any exception that occurred while retrieving the snapshot.
        /// </param>
        protected override void OnChangeReady(IDocumentSnapshot? snapshot, Exception? error)
        {
            if (snapshot == null) return;
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

        /// <summary>
        /// Called when a snapshot of the main game document changes in the database.
        /// Handles opponent moves, updates shapes queues, and starts the opponent timer if needed.
        /// </summary>
        /// <param name="snapshot">
        /// The Firestore document snapshot representing the current game state.
        /// May be null if an error occurred or document is missing.
        /// </param>
        /// <param name="error">
        /// Any exception that occurred while retrieving the snapshot.
        /// </param>
        protected override void OnChangeGame(IDocumentSnapshot? snapshot, Exception? error)
        {
            if (snapshot == null) return;
            switch (snapshot.Get<string>(Keys.ChangeKey))
            {
                case Keys.CurrentShapeMapKey:
                    ProcessShapeChange(snapshot); break;
                case Keys.PlayerMovesKey:
                    ProcessMoveChange(snapshot); break;
                case Keys.ResetKey:
                    break;// no moves or the player who made the move is the current player, so ignore.
            }
        }

        protected override void ProcessShapeChange(IDocumentSnapshot snapshot)
        {
            if (GameBoard == null || GameBoard.ShapesQueue == null ||
                    OpGameBoard == null || OpGameBoard.ShapesQueue == null ||
                    GameBoard.CurrentShape == null || OpGameBoard.CurrentShape == null) return;

            Shape newShape = fbd.CreateShape(snapshot);
            Shape newShape2 = newShape.Duplicate();
            GameBoard.ShapesQueue.Insert(newShape);
            OpGameBoard.ShapesQueue.Insert(newShape2);
        }

        protected override async void ProcessMoveChange(IDocumentSnapshot snapshot)
        {
            bool found = false;
            for (DesiredIndex = 0; DesiredIndex < MaxPlayersCount && !found; DesiredIndex++)
                if (snapshot.Get<bool>(Keys.PlayerDetailsKey + DesiredIndex +
                    TechnicalConsts.DotSign + Keys.IsShapeAtBottomKey))
                    found = true;
            DesiredIndex--;

            if (snapshot.Get<string>(Keys.PlayerDetailsKey + DesiredIndex + TechnicalConsts.DotSign
                    + Keys.PlayerIdKey) != User.UserID)// Op player has moved
            {
                Dictionary<string, string> playerMoveMap = snapshot.Get<Dictionary<string, string>>
                    (Keys.PlayerDetailsKey + DesiredIndex + TechnicalConsts.DotSign + Keys.PlayerMovesKey) ?? [];

                CurrentMovingOpId = snapshot.Get<string>(Keys.PlayerDetailsKey + DesiredIndex +
                    TechnicalConsts.DotSign + Keys.PlayerIdKey)!;

                fbd.ResetIsShapeAtBottom(GameID, DesiredIndex);

                IsMovesQueueSorting = true;

                foreach (KeyValuePair<string, string> move in playerMoveMap)
                    MovesQueue.Insert(move);
                await MovesQueue.SortByUnixTimestampKeyAsync();

                IsMovesQueueSorting = false;

                if (OpFallTimer != null && !OpFallTimer.IsRunning)
                    OpFallTimer.Start();
            }
        }

        /// <summary>
        /// Called when a snapshot of the waiting room document changes.
        /// Updates the current players count and fetches the updated list of players.
        /// </summary>
        /// <param name="snapshot">
        /// The Firestore document snapshot representing the waiting room state.
        /// May be null if an error occurred or document is missing.
        /// </param>
        /// <param name="error">
        /// Any exception that occurred while retrieving the snapshot.
        /// </param>
        protected override void OnChangeWaitingRoom(IDocumentSnapshot? snapshot, Exception? error)
        {
            if (snapshot == null) return;
            CurrentPlayersCount = snapshot.Get<int>(Keys.CurrentPlayersCountKey);
            fbd.GetPlayersFromDocument(GameID, OnCompleteChange);
        }

        /// <summary>
        /// Callback that is executed after fetching all users in the waiting room.
        /// Updates the internal UsersInGame collection and triggers UI events.
        /// </summary>
        /// <param name="users">
        /// The list of <see cref="User"/> objects currently in the game.
        /// </param>
        protected override void OnCompleteChange(ObservableCollection<User> users)
        {
            UsersInGame.Clear();
            foreach (User user in users) { UsersInGame.Add(user); }

            if (!IsFull)
                OnPlayersChange?.Invoke(this, EventArgs.Empty);
            else
            {
                if (GameBoard == null || OpGameBoard == null || User == null) return;
                fbd.SetGameIsFull(GameID);
                GameBoard.GameID = GameID;
                OpGameBoard.GameID = GameID;
                GameBoard.User = User;
                foreach (User user in UsersInGame)
                    if (user.UserID != User.UserID)
                        OpGameBoard.User = user;
                OnGameFull?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Applies the next move from the opponent's moves queue.
        /// Called on each tick of the opponent's fall timer (OpFallTimer)
        /// </summary>
        /// <param name="sender">
        /// The source of the timer event. Usually the <see cref="DispatcherTimer"/> instance.
        /// </param>
        /// <param name="e">
        /// Event arguments for the timer tick event. Usually <see cref="EventArgs.Empty"/>.
        /// </param>
        protected override void ApplyOpMove(object? sender, EventArgs e)
        {
            if (GameBoard == null || GameBoard.User == null || OpFallTimer == null) return;
            if (!MovesQueue.IsEmpty() && !IsMovesQueueSorting && GameBoard.User.UserID != CurrentMovingOpId)
            // third check is unnecessary since it enters the second if in the
            // on change game only if its not the player who finished a moved.
            {
                string move = MovesQueue.Remove().Value;
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
                    case Keys.SnapDownKey:
                        SnapDownOpShape();
                        break;
                }
            }
            else
                OpFallTimer.Stop();
        }

        /// <summary>
        /// Moves the opponent's shape to the right, if possible.
        /// </summary>
        protected override void MoveRightOpShape()
        {
            if (OpGameBoard == null) return;
            OpGameBoard.MoveRightShape();
        }

        /// <summary>
        /// Moves the opponent's shape to the left, if possible.
        /// </summary>
        protected override void MoveLeftOpShape()
        {
            if (OpGameBoard == null) return;
            OpGameBoard.MoveLeftShape();
        }

        /// <summary>
        /// Moves the opponent's shape down, if possible.
        /// </summary>
        protected override void MoveDownOpShape()
        {
            if (OpGameBoard == null) return;
            OpGameBoard.MoveDownShape();
        }

        /// <summary>
        /// Moves the opponent's shape down, if possible.
        /// </summary>
        protected override async void SnapDownOpShape()
        {
            if (OpGameBoard == null) return;
            await OpGameBoard.SnapDownShape();
        }

        /// <summary>
        /// Rotates the opponent's shape, applying wall kicks if necessary.
        /// </summary>
        protected override void RotateOpShape()
        {
            if (OpGameBoard == null) return;
            OpGameBoard.RotateShape();
        }
        #endregion
    }
}
