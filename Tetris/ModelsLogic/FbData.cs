using Firebase.Auth;
using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using Tetris.Interfaces;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    /// <summary>
    /// Provides methods for user authentication, user data management, and multiplayer game session handling using
    /// Firebase services.
    /// </summary>
    public class FbData : FbDataModel, IFbData
    {
        #region Public Methods

        /// <summary>
        /// Creates a new user with the specified email, password, and username, registers the user in Firebase, stores
        /// user data in Firestore, sends a verification email, and invokes a callback upon completion.
        /// </summary>
        /// <param name="email">The email address for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="userName">The username for the new user.</param>
        /// <param name="OnCompleteRegister">A callback function invoked when registration
        /// is complete, receiving the registration task.</param>
        /// <returns>True if the user was created and registered successfully; otherwise, false.</returns>
        public override async Task<bool> CreateUserWithEmailAndPWAsync(
            string email, string password, string userName, Func<Task, bool> OnCompleteRegister)
        {
            Task<Firebase.Auth.UserCredential> firebaseTask = 
                facl.CreateUserWithEmailAndPasswordAsync(email, password, userName);
            bool success;
            try
            {
                UserCredential credential = await firebaseTask;
                _ = facl.SignInWithEmailAndPasswordAsync(email, password);

                string userId = facl.User.Uid;
                _ = fs.Collection(Keys.UsersCollectionName).Document(userId).SetAsync(new
                {
                    userName,
                    email,
                    dateJoined = DateTime.Now.ToString(TechnicalConsts.DateFormat),
                    gamesPlayed = 0,
                    highestScore = 0,
                    totalLinesCleared = 0,
                });
                
                string idToken = await facl.User.GetIdTokenAsync(); 

                using HttpClient http = new();
                object payload = new
                {
                    requestType = Keys.VerifyEmailKey,
                    idToken
                };

                HttpResponseMessage res = await http.PostAsJsonAsync(
                    Keys.FbPostRequest + Keys.FbApiKey,
                    payload);
            }
            catch (Exception ex)
            {
                TaskCompletionSource<Firebase.Auth.UserCredential> tcs = new();
                tcs.SetException(ex);
                firebaseTask = tcs.Task;
            }
            finally
            {
                success = OnCompleteRegister(firebaseTask);
            }
            return success;
        }
        
        /// <summary>
        /// Attempts to sign in a user with the specified email and password, verifying email status and invoking a
        /// completion callback.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="OnCompleteLogin">A callback function invoked upon completion
        /// of the sign-in attempt, receiving the sign-in task.</param>
        /// <returns>A task representing the asynchronous operation, containing true 
        /// if sign-in was successful; otherwise, false.</returns>
        public override async Task<bool> SignInWithEmailAndPWAsync(
            string email, string password, Func<Task, Task<bool>> OnCompleteLogin)
        {
            Task<UserCredential> firebaseTask =
                facl.SignInWithEmailAndPasswordAsync(email, password);

            try
            {
                UserCredential credential = await firebaseTask;

                if (!credential.User.Info.IsEmailVerified)
                {
                    facl.SignOut();
                    throw new Exception(Strings.EmailVerificationError);
                }
            }
            catch (Exception ex)
            {
                TaskCompletionSource<UserCredential> tcs = new();
                tcs.SetException(ex);
                firebaseTask = tcs.Task;
            }
            return await OnCompleteLogin(firebaseTask);
        }
        
        /// <summary>
        /// Signs out the current user if a user is present.
        /// </summary>
        public override void SignOut()
        {
            if (facl != null && facl.User != null)
                facl.SignOut();
        }
        
        /// <summary>
        /// Sends a password reset email to the specified address and invokes a callback upon completion.
        /// </summary>
        /// <param name="email">The email address to which the password reset email will be sent.</param>
        /// <param name="OnCompleteSendEmail">A callback action to be invoked when the email sending task completes.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task SendPasswordResetEmailAsync(
            string email, Action<Task> OnCompleteSendEmail)
        {
            // Start Firebase sign-in
            Task firebaseTask = facl.ResetEmailPasswordAsync(email);
            try
            {
                // Await Firebase sign-in
                await firebaseTask;
            }
            catch (Exception ex)
            {
                // Wrap the exception in a Task to pass to the callback
                TaskCompletionSource<Firebase.Auth.UserCredential> tcs = new();
                tcs.SetException(ex);
                firebaseTask = tcs.Task;
            }
            finally
            {
                // Always invoke the callback, even if the sign-in failed
                OnCompleteSendEmail(firebaseTask);
            }
        }
        
        /// <summary>
        /// Retrieves the unique identifier of the current user.
        /// </summary>
        /// <returns>The user ID if available; otherwise, an empty string.</returns>
        public override string GetCurrentUserID()
        {
            return facl.User?.Uid ?? string.Empty;
        }

        /// <summary>
        /// Asynchronously retrieves user data of the specified type from Firestore using the given key.
        /// </summary>
        /// <typeparam name="T">The type of the user data to retrieve.</typeparam>
        /// <param name="key">The key identifying the user data to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, with the user data of type T if found; otherwise, the
        /// default value for type T.</returns>
        public override async Task<T> GetUserDataAsync<T>(string key)
        {
            T result = default!;

            if (!string.IsNullOrEmpty(facl.User?.Uid))
            {
                IDocumentSnapshot snapshot = await fs
                    .Collection(Keys.UsersCollectionName)
                    .Document(facl.User.Uid)
                    .GetAsync();

                if (snapshot.Exists)
                {
                    T? value = snapshot.Get<T>(key);
                    if (value != null)
                        result = value;
                }
            }

            return result;
        }

        /// <summary>
        /// Extracts and interprets Firebase error messages from a failed Task, mapping them to user-friendly messages.
        /// </summary>
        /// <param name="task">The Task containing the Firebase operation result and potential error information.</param>
        /// <returns>A user-friendly error message corresponding to the Firebase error.</returns>
        public override string IdentifyFireBaseError(Task task)
        {
            Exception ex = task.Exception!.InnerException!;
            string errorMessage = ex.Message;

            if (ex != null)
            {
                try
                {
                    int responseIndex = ex.Message.IndexOf(TechnicalConsts.ResponseText);
                    if (responseIndex >= 0)
                    {
                        // Take everything after "Response:"
                        string jsonPart = ex.Message[(responseIndex + 
                            TechnicalConsts.ResponseText.Length)..].Trim();

                        // Some Firebase responses might have extra closing braces, remove trailing stuff
                        int lastBrace = jsonPart.LastIndexOf(TechnicalConsts.ClosingBraceSign);
                        if (lastBrace >= 0)
                            jsonPart = jsonPart[..(lastBrace + 1)];

                        // Parse JSON
                        JsonDocument json = JsonDocument.Parse(jsonPart);

                        JsonElement errorElem = json.RootElement.
                            GetProperty(TechnicalConsts.ErrorJson);
                        string firebaseMessage = errorElem.
                            GetProperty(TechnicalConsts.MessageJson).ToString();

                        errorMessage = firebaseMessage switch
                        {
                            Keys.EmailExistsErrorKey => Strings.EmailExistsError,
                            Keys.OperationNotAllowedErrorKey => Strings.OperationNotAllowedError,
                            Keys.WeakPasswordErrorKey => Strings.WeakPasswordError,
                            Keys.MissingEmailErrorKey => Strings.MissingEmailError,
                            Keys.MissingPasswordErrorKey => Strings.MissingPasswordError,
                            Keys.InvalidEmailErrorKey => Strings.InvalidEmailError,
                            Keys.InvalidCredentialsErrorKey => Strings.InvalidCredentialsError,
                            Keys.UserDisabledErrorKey => Strings.UserDisabledError,
                            Keys.ManyAttemptsErrorKey => Strings.ManyAttemptsError,
                            _ => Strings.DefaultError,
                        };
                    }
                }
                catch
                {
                    errorMessage = Strings.FailedJsonError;
                }
            }
            return errorMessage;
        }
        
        /// <summary>
        /// Adds a new game entry to the database with the specified game and player details.
        /// </summary>
        /// <param name="userID">The user ID of the player creating the game.</param>
        /// <param name="creatorName">The name of the game creator.</param>
        /// <param name="cubeColor">The color of the cube for the game.</param>
        /// <param name="currentPlayersCount">The current number of players in the game.</param>
        /// <param name="maxPlayersCount">The maximum number of players allowed in the game.</param>
        /// <param name="isFull">Indicates whether the game is full.</param>
        /// <param name="firstShapeId">The ID of the current shape.</param>
        /// <param name="firstShapeInGameId">The in-game ID of the current shape.</param>
        /// <param name="firstShapeColor">The color of the current shape.</param>
        /// <param name="secondShapeId">The ID of the next shape.</param>
        /// <param name="secondShapeInGameId">The in-game ID of the next shape.</param>
        /// <param name="secondShapeColor">The color of the next shape.</param>
        /// <param name="isPublicGame">Indicates whether the game is public.</param>
        /// <returns>The auto-generated document ID of the newly created game entry.</returns>
        public override string AddGameToDB(string userID, string creatorName, string cubeColor,
            int currentPlayersCount, int maxPlayersCount, bool isFull, int firstShapeId,
            int firstShapeInGameId, string firstShapeColor, int secondShapeId,
            int secondShapeInGameId, string secondShapeColor, bool isPublicGame)
        {
            // Create a new document reference with an auto-generated ID
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document();

            docRef.SetAsync(new
            {
                CreatorName = creatorName,
                CubeColor = cubeColor,
                CurrentPlayersCount = currentPlayersCount,
                MaxPlayersCount = maxPlayersCount,
                IsFull = isFull,
                IsPublicGame = isPublicGame,
                TimeCreated = DateTime.UtcNow,
                Changed = string.Empty
            });

            docRef.UpdateAsync(new Dictionary<string, object>
            {
                {
                    Keys.CurrentShapeMapKey, new Dictionary<string, object>
                    {
                        { Keys.ShapeIdKey, firstShapeId },
                        { Keys.ShapeInGameIdKey, firstShapeInGameId },
                        { Keys.ShapeColorKey, firstShapeColor }
                    }
                },
                {
                    Keys.NextShapeMapKey, new Dictionary<string, object>
                    {
                        { Keys.ShapeIdKey, secondShapeId },
                        { Keys.ShapeInGameIdKey, secondShapeInGameId },
                        { Keys.ShapeColorKey, secondShapeColor }
                    }
                }
            });

            for (int i = 0; i < maxPlayersCount; i++)
            { 
                string currentUserId = i == 0 ? userID : string.Empty;
                docRef.UpdateAsync(new Dictionary<string, object>
                {
                    {
                        Keys.PlayerDetailsKey + i, new Dictionary<string, object>
                        {
                            { Keys.PlayerIdKey, currentUserId },
                            { Keys.IsShapeAtBottomKey, false },
                            { Keys.IsPlayerReadyKey, false },
                            { Keys.PlayerMovesKey, new Dictionary<string, object> { } }
                        }
                    }
                });
            }

            // Return the auto-generated document ID as a Task<string>
            return docRef.Id;
        }
        
        /// <summary>
        /// Adds a listener to the games collection that triggers when the collection changes.
        /// </summary>
        /// <param name="OnChange">Handler invoked when the games collection snapshot changes.</param>
        /// <returns>A registration object for managing the listener's lifecycle.</returns>
        public override IListenerRegistration AddGamesCollectionListener(
            Plugin.CloudFirestore.QuerySnapshotHandler OnChange)
        {
            ICollectionReference cr = fs.Collection(Keys.GamesCollectionName);
            return cr.AddSnapshotListener(OnChange);
        }
      
        /// <summary>
        /// Asynchronously retrieves the available games and invokes the specified callback with the resulting
        /// collection.
        /// </summary>
        /// <param name="onCompleteChange">Callback invoked with the updated collection of available games.</param>
        public override async void GetAvailGames(Action<ObservableCollection<Game>> onCompleteChange)
        {
            ObservableCollection<Game> newList = await GetAvailGamesList();
            onCompleteChange(newList);
        }
      
        /// <summary>
        /// Retrieves a list of available public games that are not full from the database.
        /// </summary>
        /// <returns>An observable collection of Game objects representing available games.</returns>
        public override async Task<ObservableCollection<Game>> GetAvailGamesList()
        {
            ICollectionReference collectionRef = fs.Collection(Keys.GamesCollectionName);
            IQuerySnapshot snapshot = await collectionRef.WhereEqualsTo(Keys.IsFullKey, false)
                .WhereEqualsTo(Keys.IsPublicGameKey, true).GetAsync();
            ObservableCollection<Game> newList = [];

            foreach (IDocumentSnapshot doc in snapshot.Documents)
            {
                Game game = new(
                    doc.Get<string>(Keys.CubeColorKey)!,
                    doc.Get<string>(Keys.CreatorNameKey)!,
                    doc.Get<int>(Keys.CurrentPlayersCountKey),
                    doc.Get<int>(Keys.MaxPlayersCountKey),
                    doc.Get<bool>(Keys.IsPublicGameKey),
                    new Shape(doc.Get<int>(Keys.CurrentShapeMapKey +
                    TechnicalConsts.DotSign + Keys.ShapeIdKey),
                        doc.Get<int>(Keys.CurrentShapeMapKey + 
                        TechnicalConsts.DotSign + Keys.ShapeInGameIdKey),
                        doc.Get<string>(Keys.CurrentShapeMapKey + 
                        TechnicalConsts.DotSign + Keys.ShapeColorKey)!),
                    new Shape(doc.Get<int>(Keys.NextShapeMapKey +
                    TechnicalConsts.DotSign + Keys.ShapeIdKey),
                        doc.Get<int>(Keys.NextShapeMapKey + 
                        TechnicalConsts.DotSign + Keys.ShapeInGameIdKey),
                        doc.Get<string>(Keys.NextShapeMapKey + 
                        TechnicalConsts.DotSign + Keys.ShapeColorKey)!),
                    doc.Id
                );
                newList.Add(game);
            }
            return newList;
        }
    
        /// <summary>
        /// Handles the logic for when a player leaves a game room by decrementing the current player count and clearing
        /// the player's details.
        /// </summary>
        /// <param name="id">The unique identifier of the game room.</param>
        /// <param name="leavingUserID">The user ID of the player who is leaving.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task OnPlayerLeaveWR(string id, string leavingUserID)
        {
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(id);
            IDocumentSnapshot docSnap = await docRef.GetAsync();
            _ = docRef.UpdateAsync(Keys.CurrentPlayersCountKey, FieldValue.Increment(-1));
            for (int i = 0; i < docSnap.Get<int>(Keys.MaxPlayersCountKey); i++)
                if (docSnap.Get<string>(Keys.PlayerDetailsKey + i + 
                    TechnicalConsts.DotSign + Keys.PlayerIdKey) == leavingUserID)
                    _ = docRef.UpdateAsync(Keys.PlayerDetailsKey + i + 
                        TechnicalConsts.DotSign + Keys.PlayerIdKey, string.Empty);
        }
  
        /// <summary>
        /// Handles a player joining a game by incrementing the current player count and assigning the joining user's ID
        /// to the first available player slot.
        /// </summary>
        /// <param name="id">The unique identifier of the game document.</param>
        /// <param name="joiningUserID">The user ID of the player joining the game.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task OnPlayerJoinWR(string id, string joiningUserID)
        {
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(id);
            IDocumentSnapshot docSnap = await docRef.GetAsync();
            _ = docRef.UpdateAsync(Keys.CurrentPlayersCountKey, FieldValue.Increment(1));
            bool addedOnce = false;
            for (int i = 0; i < docSnap.Get<int>(Keys.MaxPlayersCountKey) && !addedOnce; i++)
                if (docSnap.Get<string>(Keys.PlayerDetailsKey + i + 
                    TechnicalConsts.DotSign + Keys.PlayerIdKey) == string.Empty)
                {
                    addedOnce = true;
                    _ = docRef.UpdateAsync(Keys.PlayerDetailsKey + i + 
                        TechnicalConsts.DotSign + Keys.PlayerIdKey, joiningUserID);
                }
        }
     
        /// <summary>
        /// Removes a game document from the database using the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the game to delete.</param>
        public override void DeleteGameFromDB(string id)
        {
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(id);
            docRef.DeleteAsync();
        }
   
        /// <summary>
        /// Retrieves the list of players for the specified game from the database and invokes the callback with the
        /// resulting collection.
        /// </summary>
        /// <param name="gameID">The unique identifier of the game whose players are to be retrieved.</param>
        /// <param name="onCompleteChange">The callback to invoke with the collection of retrieved users.</param>
        public override async void GetPlayersFromDocument(string gameID, 
            Action<ObservableCollection<User>> onCompleteChange)
        {
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            IDocumentSnapshot docSnap = await docRef.GetAsync();
            ObservableCollection<User> newList = [];
            int maxPlayersCount = docSnap.Get<int>(Keys.MaxPlayersCountKey);
            List<string> playerIds = [];
            for (int i = 0; i < maxPlayersCount; i++)
            {
                string playerIdScheme = Keys.PlayerDetailsKey + i + 
                    TechnicalConsts.DotSign + Keys.PlayerIdKey;
                playerIds.Add(docSnap.Get<string>(playerIdScheme)!);
            }
            for (int i = 0; i < maxPlayersCount; i++)
            {
                if (!string.IsNullOrEmpty(playerIds[i]))
                {
                    User tempUser = await UserIDToObject(playerIds[i]);
                    newList.Add(tempUser);
                }
            }
            onCompleteChange(newList);
        }
     
        /// <summary>
        /// Asynchronously retrieves the current number of players for the specified game.
        /// </summary>
        /// <param name="gameID">The unique identifier of the game.</param>
        /// <returns>A task representing the asynchronous operation, containing the current player count.</returns>
        public override async Task<int> GetCurrentPlayersCount(string gameID)
        {
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            IDocumentSnapshot docSnap = await docRef.GetAsync();
            return docSnap.Get<int>(Keys.CurrentPlayersCountKey);
        }
 
        /// <summary>
        /// Marks the specified game as full in the database.
        /// </summary>
        /// <param name="gameID">The unique identifier of the game to update.</param>
        public override void SetGameIsFull(string gameID)
        {
            IDocumentReference documentReference = fs.Collection(
                Keys.GamesCollectionName).Document(gameID);
            documentReference.UpdateAsync(Keys.IsFullKey, true);
        }
    
        /// <summary>
        /// Updates the current shape information for a specified game in the database.
        /// </summary>
        /// <param name="currentShape">The shape to add to the game.</param>
        /// <param name="gameId">The identifier of the game to update.</param>
        public override void AddShape(Shape currentShape, string gameId)
        {
            if (currentShape.Color == null) return;
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(gameId);
            docRef.UpdateAsync(new Dictionary<object, object>
            {
                { Keys.CurrentShapeMapKey, new Dictionary<string, object>
                    {
                        { Keys.ShapeIdKey, currentShape.Id },
                        { Keys.ShapeInGameIdKey, currentShape.InGameId },
                        { Keys.ShapeColorKey, Converters.
                        StringAndColorConverter.ColorToColorName(currentShape.Color) }
                    }
                },
                { Keys.ChangeKey, Keys.CurrentShapeMapKey }
            });
        }
 
        /// <summary>
        /// Creates a Shape instance using data retrieved from the provided document snapshot.
        /// </summary>
        /// <param name="snapshot">The document snapshot containing shape data.</param>
        /// <returns>A new Shape object initialized with values from the snapshot.</returns>
        public override Shape CreateShape(IDocumentSnapshot snapshot)
        {
            return new Shape(
                snapshot.Get<int>(Keys.CurrentShapeMapKey +
                TechnicalConsts.DotSign + Keys.ShapeIdKey)!,
                snapshot.Get<int>(Keys.CurrentShapeMapKey + 
                TechnicalConsts.DotSign + Keys.ShapeInGameIdKey)!,
                snapshot.Get<string>(Keys.CurrentShapeMapKey +
                TechnicalConsts.DotSign + Keys.ShapeColorKey)!);
        }
  
        /// <summary>
        /// Finalizes the current round for a player by recording their moves and updating their game state in the
        /// database.
        /// </summary>
        /// <param name="userID">The unique identifier of the player finishing the round.</param>
        /// <param name="gameID">The unique identifier of the game.</param>
        /// <param name="movesQueue">A queue containing the moves made by the player during the round.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task FinishRound(
            string userID, string gameID, Queue<string> movesQueue)
        {
            IDocumentReference dr = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            IDocumentSnapshot currentSnapshot = await dr.GetAsync();
            int desiredIndex, maxPlayers = currentSnapshot.Get<int>(Keys.MaxPlayersCountKey);
            List<string> ids = [];
            bool found = false;

            for (int i = 0; i < maxPlayers; i++)// 1
                ids.Add(currentSnapshot.Get<string>(Keys.PlayerDetailsKey + i + 
                    TechnicalConsts.DotSign + Keys.PlayerIdKey) ?? string.Empty);
            for (desiredIndex = 0; desiredIndex < maxPlayers && !found; desiredIndex++)// 2
                if (ids[desiredIndex] == userID)
                    found = true;
            desiredIndex--;

            Queue<string> tempMovesQueue = new();
            Dictionary<string, string> moves = [];
            while (!movesQueue.IsEmpty())
            {
                string move = movesQueue.Remove();
                tempMovesQueue.Insert(move);
                moves.Add(DateTimeOffset.UtcNow.
                    ToUnixTimeMilliseconds().ToString(), move);
                Thread.Sleep(1);
            }

            Dictionary<string, object> updates = new()
            {
                { Keys.ChangeKey, Keys.PlayerMovesKey },
                {
                    Keys.PlayerDetailsKey + desiredIndex + 
                    TechnicalConsts.DotSign + Keys.PlayerMovesKey, moves
                },
                {
                    Keys.PlayerDetailsKey + desiredIndex + 
                    TechnicalConsts.DotSign + Keys.IsShapeAtBottomKey, true
                }
            };

            _ = fs.Collection(Keys.GamesCollectionName).
                Document(gameID).UpdateAsync(updates);
        }

        /// <summary>
        /// Registers a real-time listener for changes to a specific game document in the database.
        /// </summary>
        /// <param name="gameID">The unique identifier of the game to monitor.</param>
        /// <param name="OnChange">The handler that will be invoked whenever the game document changes.</param>
        /// <returns>
        /// An IListenerRegistration that can be used to stop listening for updates,
        /// or null if the listener could not be created.
        /// </returns>
        public override IListenerRegistration? AddGameListener(string gameID,
            Plugin.CloudFirestore.DocumentSnapshotHandler OnChange)
        {
            IDocumentReference dr = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            return dr.AddSnapshotListener(OnChange);
        }

        /// <summary>
        /// Marks a specific player as ready within a game by updating their readiness status
        /// in the database.
        /// </summary>
        /// <param name="gameID">The unique identifier of the game.</param>
        /// <param name="maxPlayersCount">The maximum number of players allowed in the game.</param>
        /// <param name="userID">The unique identifier of the player to mark as ready.</param>
        public override async void SetPlayerReady(
            string gameID, int maxPlayersCount, string userID)
        {
            IDocumentReference dr = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            IDocumentSnapshot snapshot = await dr.GetAsync();
            bool changed = false;
            for (int i = 0; i < maxPlayersCount && !changed; i++)
                if (snapshot.Get<string>(Keys.PlayerDetailsKey + i + 
                    TechnicalConsts.DotSign + Keys.PlayerIdKey) == userID)
                {
                    _ = dr.UpdateAsync(Keys.PlayerDetailsKey + i + 
                        TechnicalConsts.DotSign + Keys.IsPlayerReadyKey, true);
                    changed = true;
                }
        }

        /// <summary>
        /// Resets the "IsShapeAtBottom" status for a specific player in a game
        /// by updating the corresponding field in the database.
        /// </summary>
        /// <param name="gameID">The unique identifier of the game.</param>
        /// <param name="desiredIndex">The index of the player whose shape status should be reset.</param>
        public override void ResetIsShapeAtBottom(string gameID, int desiredIndex)
        {
            IDocumentReference dr = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            dr.UpdateAsync(new Dictionary<string, object>
            {
                {Keys.PlayerDetailsKey + desiredIndex
                + TechnicalConsts.DotSign + Keys.IsShapeAtBottomKey, false },
                {Keys.ChangeKey, Keys.ResetKey }
            });
        }

        /// <summary>
        /// Deletes game documents from the database that were created before a specified cutoff time.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous deletion operation.
        /// </returns>
        public override async Task DeleteFbDocsAsync()
        {
            try
            {
                ICollectionReference collectionRef = fs.Collection(Keys.GamesCollectionName);

                // Calculate the cutoff time
                DateTime cutoffTime = DateTime.UtcNow - 
                    TimeSpan.FromSeconds(ConstData.TimePassedToDeleteFbDocS);

                // If your TimeCreatedKey is stored as a Firestore Timestamp, you can query directly
                IQuery query = collectionRef.WhereLessThan(Keys.TimeCreatedKey, cutoffTime);
                IQuerySnapshot snapshot = await query.GetAsync();
                foreach (IDocumentSnapshot docSnap in snapshot.Documents)
                {
                    try { await docSnap.Reference.DeleteAsync(); }
                    catch { }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Updates the user's post-game statistics both locally and in the database.
        /// </summary>
        /// <param name="user">
        /// The <see cref="User"/> object containing the updated statistics to persist.
        /// </param>
        public override void UpdateUserPostGame(User user)
        {
            Preferences.Set(Keys.GamesPlayedKey, user.GamesPlayed);
            Preferences.Set(Keys.HighestScoreKey, user.HighestScore);
            Preferences.Set(Keys.TotalLinesKey, user.TotalLines);

            IDocumentReference dr = fs.Collection(
                Keys.UsersCollectionName).Document(user.UserID);
            Dictionary<string, object> updates = new()
            {
                { Keys.GamesPlayedKey, user.GamesPlayed },
                { Keys.HighestScoreKey, user.HighestScore },
                { Keys.TotalLinesKey, user.TotalLines }
            };
            _ = dr.UpdateAsync(updates);
        }

        /// <summary>
        /// Attempts to assign a private join code to a game, ensuring that the code
        /// is not already in use by another game.
        /// </summary>
        /// <param name="gameID">The unique identifier of the game.</param>
        /// <param name="code">The private join code to assign to the game.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result is true if the
        /// code was successfully assigned; otherwise, false if the code is already in use.
        /// </returns>
        public override async Task<bool> SetPrivateJoinCode(string gameID, int code)
        {
            bool result = true;
            ICollectionReference collectionRef = fs.Collection(Keys.GamesCollectionName);
            IQuerySnapshot snapshot = await collectionRef
                .WhereEqualsTo(Keys.PrivateJoinCodeKey, code)
                .GetAsync();
            if (snapshot.Documents.Any())
                result = false;
            else
            {
                IDocumentReference dr = fs
                    .Collection(Keys.GamesCollectionName)
                    .Document(gameID);
                _ = dr.UpdateAsync(Keys.PrivateJoinCodeKey, code);
            }
            return result;
        }

        /// <summary>
        /// Retrieves a game from the database based on its private join code.
        /// </summary>
        /// <param name="code">The private join code associated with the game.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result is the corresponding
        /// <see cref="Game"/> if found; otherwise, null if no game matches the provided code.
        /// </returns>
        public override async Task<Game> GetGameByCode(int code)
        {
            Game? result = null;
            ICollectionReference collectionRef =
                fs.Collection(Keys.GamesCollectionName);
            IQuerySnapshot querySnap = await collectionRef
                .WhereEqualsTo(Keys.PrivateJoinCodeKey, code)
                .GetAsync();
            IDocumentSnapshot? docSnap =
                querySnap.Documents.FirstOrDefault();

            if (docSnap != null)
            {
                result = new Game(
                    docSnap.Get<string>(Keys.CubeColorKey)!,
                    docSnap.Get<string>(Keys.CreatorNameKey)!,
                    docSnap.Get<int>(Keys.CurrentPlayersCountKey),
                    docSnap.Get<int>(Keys.MaxPlayersCountKey),
                    docSnap.Get<bool>(Keys.IsPublicGameKey),
                    new Shape(
                        docSnap.Get<int>(
                            Keys.CurrentShapeMapKey +
                            TechnicalConsts.DotSign +
                            Keys.ShapeIdKey),

                        docSnap.Get<int>(
                            Keys.CurrentShapeMapKey +
                            TechnicalConsts.DotSign +
                            Keys.ShapeInGameIdKey),

                        docSnap.Get<string>(
                            Keys.CurrentShapeMapKey +
                            TechnicalConsts.DotSign +
                            Keys.ShapeColorKey)!),
                    new Shape(
                        docSnap.Get<int>(
                            Keys.NextShapeMapKey +
                            TechnicalConsts.DotSign +
                            Keys.ShapeIdKey),

                        docSnap.Get<int>(
                            Keys.NextShapeMapKey +
                            TechnicalConsts.DotSign +
                            Keys.ShapeInGameIdKey),

                        docSnap.Get<string>(
                            Keys.NextShapeMapKey +
                            TechnicalConsts.DotSign +
                            Keys.ShapeColorKey)!),
                    docSnap.Id
                );
            }

            return result!;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Retrieves a user document from the database by their unique ID and converts it
        /// into a <see cref="User"/> object.
        /// </summary>
        /// <param name="id">The unique identifier of the user to retrieve.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result is the <see cref="User"/>
        /// object corresponding to the specified ID.
        /// </returns>
        protected override async Task<User> UserIDToObject(string id)
        {
            IDocumentSnapshot docSnap = await fs.Collection(
                Keys.UsersCollectionName).Document(id).GetAsync();
            User user = new()
            {
                UserID = id,
                UserName = docSnap.Get<string>(Keys.UserNameKey)!,
                Email = docSnap.Get<string>(Keys.EmailKey)!,
                DateJoined = docSnap.Get<string>(Keys.DateJoinedKey)!,
                GamesPlayed = docSnap.Get<int>(Keys.GamesPlayedKey),
                HighestScore = docSnap.Get<int>(Keys.HighestScoreKey),
                TotalLines = docSnap.Get<int>(Keys.TotalLinesKey)
            };
            return user;
        }
        #endregion
    }
}
