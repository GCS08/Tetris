using Firebase.Auth;
using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class FbData : FbDataModel
    {
        public override async Task<bool> CreateUserWithEmailAndPWAsync(string email, string password, string userName, Func<Task, bool> OnCompleteRegister)
        {
            Task<Firebase.Auth.UserCredential> firebaseTask = facl.CreateUserWithEmailAndPasswordAsync(email, password, userName);
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
                    settings0 = true,
                    settings1 = true,
                    settings2 = true,
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
        public override async Task<bool> SignInWithEmailAndPWAsync(string email, string password, Func<Task, Task<bool>> OnCompleteLogin)
        {
            Task<UserCredential> firebaseTask = facl.SignInWithEmailAndPasswordAsync(email, password);

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
        public override void SignOut()
        {
            if (facl != null && facl.User != null)
                facl.SignOut();
        }
        public override async Task SendPasswordResetEmailAsync(string email, Action<Task> OnCompleteSendEmail)
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
        public override string GetCurrentUserID()
        {
            return facl.User?.Uid ?? string.Empty;
        }
        public override async Task<T> GetUserDataAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(facl.User?.Uid))
                return default!;
            IDocumentSnapshot snapshot = await fs.Collection(Keys.UsersCollectionName).Document(facl.User.Uid).GetAsync();
            if (!snapshot.Exists)
                return default!;

            // Firebase Cloud Firestore supports strongly-typed Get<T>
            T? value = snapshot.Get<T>(key);
            return value != null ? value : default!;
        }
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
                        string jsonPart = ex.Message[(responseIndex + TechnicalConsts.ResponseText.Length)..].Trim();

                        // Some Firebase responses might have extra closing braces, remove trailing stuff
                        int lastBrace = jsonPart.LastIndexOf(TechnicalConsts.ClosingBraceSign);
                        if (lastBrace >= 0)
                            jsonPart = jsonPart[..(lastBrace + 1)];

                        // Parse JSON
                        JsonDocument json = JsonDocument.Parse(jsonPart);

                        JsonElement errorElem = json.RootElement.GetProperty(TechnicalConsts.ErrorJson);
                        string firebaseMessage = errorElem.GetProperty(TechnicalConsts.MessageJson).ToString();

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
        public override string AddGameToDB(string userID, string creatorName, string cubeColor,
            int currentPlayersCount, int maxPlayersCount, bool isFull, int currentShapeId,
            int currentShapeInGameId, string currentShapeColor, bool isPublicGame)
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
                TimeCreated = DateTime.UtcNow.ToString()
            });

            docRef.UpdateAsync(new Dictionary<string, object>
            {
                {
                    Keys.CurrentShapeMapKey, new Dictionary<string, object>
                    {
                        { Keys.CurrentShapeIdKey, currentShapeId },
                        { Keys.CurrentShapeInGameIdKey, currentShapeInGameId },
                        { Keys.CurrentShapeColorKey, currentShapeColor }
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
        public override IListenerRegistration AddGamesCollectionListener(
            Plugin.CloudFirestore.QuerySnapshotHandler OnChange)
        {
            ICollectionReference cr = fs.Collection(Keys.GamesCollectionName);
            return cr.AddSnapshotListener(OnChange);
        }
        public override async void GetAvailGames(Action<ObservableCollection<Game>> onCompleteChange)
        {
            ObservableCollection<Game> newList = await GetAvailGamesList();
            onCompleteChange(newList);
        }
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
                    new Shape(doc.Get<int>(Keys.CurrentShapeMapKey + TechnicalConsts.DotSign + Keys.CurrentShapeIdKey),
                        doc.Get<int>(Keys.CurrentShapeMapKey + TechnicalConsts.DotSign + Keys.CurrentShapeInGameIdKey),
                        doc.Get<string>(Keys.CurrentShapeMapKey + TechnicalConsts.DotSign + Keys.CurrentShapeColorKey)!),
                    doc.Id
                );
                newList.Add(game);
            }
            return newList;
        }
        public override async Task OnPlayerLeaveWR(string id, string leavingUserID)
        {
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(id);
            IDocumentSnapshot docSnap = await docRef.GetAsync();
            _ = docRef.UpdateAsync(Keys.CurrentPlayersCountKey, FieldValue.Increment(-1));
            for (int i = 0; i < docSnap.Get<int>(Keys.MaxPlayersCountKey); i++)
                if (docSnap.Get<string>(Keys.PlayerDetailsKey + i + TechnicalConsts.DotSign + Keys.PlayerIdKey) == leavingUserID)
                    _ = docRef.UpdateAsync(Keys.PlayerDetailsKey + i + TechnicalConsts.DotSign + Keys.PlayerIdKey, string.Empty);
        }
        public override async Task OnPlayerJoinWR(string id, string joiningUserID)
        {
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(id);
            IDocumentSnapshot docSnap = await docRef.GetAsync();
            _ = docRef.UpdateAsync(Keys.CurrentPlayersCountKey, FieldValue.Increment(1));
            bool addedOnce = false;
            for (int i = 0; i < docSnap.Get<int>(Keys.MaxPlayersCountKey) && !addedOnce; i++)
                if (docSnap.Get<string>(Keys.PlayerDetailsKey + i + TechnicalConsts.DotSign + Keys.PlayerIdKey) == string.Empty)
                {
                    addedOnce = true;
                    _ = docRef.UpdateAsync(Keys.PlayerDetailsKey + i + TechnicalConsts.DotSign + Keys.PlayerIdKey, joiningUserID);
                }
        }
        public override void DeleteGameFromDB(string id)
        {
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(id);
            docRef.DeleteAsync();
        }
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
                string playerIdScheme = Keys.PlayerDetailsKey + i + TechnicalConsts.DotSign + Keys.PlayerIdKey;
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
        public override async Task<int> GetCurrentPlayersCount(string gameID)
        {
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            IDocumentSnapshot docSnap = await docRef.GetAsync();
            return docSnap.Get<int>(Keys.CurrentPlayersCountKey);
        }
        protected override async Task<User> UserIDToObject(string id)
        {
            IDocumentSnapshot docSnap = await fs.Collection(Keys.UsersCollectionName).Document(id).GetAsync();
            User user = new()
            {
                UserID = id,
                UserName = docSnap.Get<string>(Keys.UserNameKey)!,
                Email = docSnap.Get<string>(Keys.EmailKey)!,
                DateJoined = docSnap.Get<string>(Keys.DateJoinedKey)!,
                GamesPlayed = docSnap.Get<int>(Keys.GamesPlayedKey),
                HighestScore = docSnap.Get<int>(Keys.HighestScoreKey),
                TotalLines = docSnap.Get<int>(Keys.TotalLinesKey),
                Settings0 = docSnap.Get<bool>(Keys.Settings0Key),
                Settings1 = docSnap.Get<bool>(Keys.Settings1Key),
                Settings2 = docSnap.Get<bool>(Keys.Settings2Key)
            };
            return user;
        }
        public override void SetGameIsFull(string gameID)
        {
            IDocumentReference documentReference = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            documentReference.UpdateAsync(Keys.IsFullKey, true);
        }
        public override void AddShape(Shape currentShape, string gameId)
        {
            System.Diagnostics.Debug.WriteLine("FbData.AddShape start");
            if (currentShape.Color == null) return;
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(gameId);
            docRef.UpdateAsync(Keys.CurrentShapeMapKey, new Dictionary<string, object>
            {
                { Keys.CurrentShapeIdKey, currentShape.Id },
                { Keys.CurrentShapeInGameIdKey, currentShape.InGameId },
                { Keys.CurrentShapeColorKey, Converters.
                StringAndColorConverter.ColorToColorName(currentShape.Color) }
            });
            System.Diagnostics.Debug.WriteLine("FbData.AddShape end");
        }
        public override Shape CreateShape(IDocumentSnapshot snapshot)
        {
            return new Shape(
                snapshot.Get<int>(Keys.CurrentShapeMapKey + TechnicalConsts.DotSign + Keys.CurrentShapeIdKey)!,
                snapshot.Get<int>(Keys.CurrentShapeMapKey + TechnicalConsts.DotSign + Keys.CurrentShapeInGameIdKey)!,
                snapshot.Get<string>(Keys.CurrentShapeMapKey + TechnicalConsts.DotSign + Keys.CurrentShapeColorKey)!);
        }
        public override async Task FinishRound(string userID, string gameID, Queue<string> movesQueue)
        {
            IDocumentReference dr = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            IDocumentSnapshot currentSnapshot = await dr.GetAsync();
            int desiredIndex;
            int maxPlayers = currentSnapshot.Get<int>(Keys.MaxPlayersCountKey);
            List<string> ids = [];
            bool found = false;

            for (int i = 0; i < maxPlayers; i++)// 1
                ids.Add(currentSnapshot.Get<string>(Keys.PlayerDetailsKey + i + TechnicalConsts.DotSign + Keys.PlayerIdKey) ?? string.Empty);
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
                {
                    Keys.PlayerDetailsKey + desiredIndex + TechnicalConsts.DotSign + Keys.PlayerMovesKey, moves
                },
                {
                    Keys.PlayerDetailsKey + desiredIndex + TechnicalConsts.DotSign + Keys.IsShapeAtBottomKey, true
                }
            };

            _ = fs.Collection(Keys.GamesCollectionName).Document(gameID).UpdateAsync(updates);
        }
        public override IListenerRegistration? AddGameListener(string gameID,
            Plugin.CloudFirestore.DocumentSnapshotHandler OnChange)
        {
            IDocumentReference dr = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            return dr.AddSnapshotListener(OnChange);
        }
        public override async void SetPlayerReady(string gameID, int maxPlayersCount, string userID)
        {
            IDocumentReference dr = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            IDocumentSnapshot snapshot = await dr.GetAsync();
            bool changed = false;
            for (int i = 0; i < maxPlayersCount && !changed; i++)
                if (snapshot.Get<string>(Keys.PlayerDetailsKey + i + TechnicalConsts.DotSign + Keys.PlayerIdKey) == userID)
                {
                    _ = dr.UpdateAsync(Keys.PlayerDetailsKey + i + TechnicalConsts.DotSign + Keys.IsPlayerReadyKey, true);
                    changed = true;
                }
        }
        public override void ResetIsShapeAtBottom(string gameID, int desiredIndex)
        {
            IDocumentReference dr = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            dr.UpdateAsync(Keys.PlayerDetailsKey + desiredIndex
                + TechnicalConsts.DotSign + Keys.IsShapeAtBottomKey, false);
        }
        public override async void DeleteFbDocs()
        {
            ICollectionReference collectionRef =
                fs.Collection(Keys.GamesCollectionName);

            IQuerySnapshot snapshot = await collectionRef.GetAsync();

            foreach (IDocumentSnapshot docSnap in snapshot.Documents)
            {
                DateTime timeCreated =
                    DateTime.Parse(docSnap.Get<string>(Keys.TimeCreatedKey)!);

                TimeSpan timeDiff = DateTime.UtcNow - timeCreated;

                if (timeDiff.TotalSeconds >= ConstData.TimePassedToDeleteFbDocS)
                {
                    _ = docSnap.Reference.DeleteAsync();
                }
            }
        }

        public void UpdateUserPostGame(User user)
        {
            IDocumentReference dr = fs.Collection(Keys.UsersCollectionName).Document(user.UserID);
            Dictionary<string, object> updates = new()
            {
                { Keys.GamesPlayedKey, user.GamesPlayed },
                { Keys.HighestScoreKey, user.HighestScore },
                { Keys.TotalLinesKey, user.TotalLines }
            };
            _ = dr.UpdateAsync(updates);
        }
    }
}
