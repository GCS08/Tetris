using System.Collections;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.Maui.ApplicationModel.Communication;
using Plugin.CloudFirestore;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class FbData : FbDataModel
    {
        public override async Task<bool> CreateUserWithEmailAndPWAsync(string email, string password, string userName, Func<Task, Task<bool>> OnCompleteRegister)
        {
            Task<Firebase.Auth.UserCredential> firebaseTask = facl.CreateUserWithEmailAndPasswordAsync(email, password, userName);
            bool success;
            try
            {
                UserCredential credential = await firebaseTask;

                // Immediately sign in the new user so Firestore writes can succeed
                await facl.SignInWithEmailAndPasswordAsync(email, password);

                string userId = facl.User.Uid;
                await fs.Collection(Keys.UsersCollectionName).Document(userId).SetAsync(new
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

                // ... inside your try block, after the user signs in
                string idToken = await facl.User.GetIdTokenAsync(); // the user’s token

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
                success = await OnCompleteRegister(firebaseTask);
            }
            return success;
        }
        public override async Task<bool> SignInWithEmailAndPWAsync(string email, string password, Func<Task, Task<bool>> OnCompleteLogin)
        {
            Task<UserCredential> firebaseTask = facl.SignInWithEmailAndPasswordAsync(email, password);

            try
            {
                // Await the Firebase sign-in
                UserCredential credential = await firebaseTask;

                if (!credential.User.Info.IsEmailVerified)
                {
                    // Immediately sign out the unverified user
                    facl.SignOut();
                    throw new Exception(Strings.EmailVerificationError);
                }

                // optional: continue if verified
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
        public override async Task SendPasswordResetEmailAsync(string email, Func<Task, Task> OnCompleteSendEmail)
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
                await OnCompleteSendEmail(firebaseTask);
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
            Exception? ex = task.Exception?.InnerException;
            string errorMessage = ex!.Message;

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
        public override async Task<string> AddGameToDB(string userID, string creatorName, string cubeColor,
            int currentPlayersCount, int maxPlayersCount, bool isFull,
            int currentShapeId, string currentShapeColor, bool isPublicGame)
        {
            // Create a new document reference with an auto-generated ID
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document();

            await docRef.SetAsync(new
            {
                Player0 = userID,
                CreatorName = creatorName,
                CubeColor = cubeColor,
                CurrentPlayersCount = currentPlayersCount,
                MaxPlayersCount = maxPlayersCount,
                IsFull = isFull,
                CurrentShapeId = currentShapeId,
                CurrentShapeColor = currentShapeColor,
                IsPublicGame = isPublicGame
            });

            for (int i = 1; i < maxPlayersCount; i++)
                await docRef.UpdateAsync(Keys.PlayerKey + i, string.Empty);

            // Return the auto-generated document ID
            return docRef.Id;
        }
        public override IListenerRegistration AddGamesCollectionListener(
            Plugin.CloudFirestore.QuerySnapshotHandler OnChange)
        {
            ICollectionReference cr = fs.Collection(Keys.GamesCollectionName);
            return cr.AddSnapshotListener(OnChange);
        }
        public override IListenerRegistration AddGameListener(
            string documentId, Plugin.CloudFirestore.DocumentSnapshotHandler OnChange)
        {
            IDocumentReference dr = fs.Collection(Keys.GamesCollectionName).Document(documentId);
            return dr.AddSnapshotListener(OnChange);
        }
        public override async void GetAvailGames(Action<ObservableCollection<Game>> onCompleteChange)
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
                    new Shape(doc.Get<int>(Keys.CurrentShapeIdKey),
                        doc.Get<string>(Keys.CurrentShapeColorKey)!),
                    doc.Id
                );
                newList.Add(game);                
            }
            onCompleteChange(newList);
        }
        public override async Task<ObservableCollection<Game>> GetAvailGames()
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
                    new Shape(doc.Get<int>(Keys.CurrentShapeIdKey),
                        doc.Get<string>(Keys.CurrentShapeColorKey)!),
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
            await docRef.UpdateAsync(Keys.CurrentPlayersCountKey, FieldValue.Increment(-1));
            for (int i = 0; i < docSnap.Get<int>(Keys.MaxPlayersCountKey); i++)
                if (docSnap.Get<string>(Keys.PlayerKey + i) == leavingUserID)
                    await docRef.UpdateAsync(Keys.PlayerKey + i, string.Empty);
        }
        public override async Task OnPlayerJoinWR(string id, string leavingUserID)
        {
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(id);
            IDocumentSnapshot docSnap = await docRef.GetAsync();
            await docRef.UpdateAsync(Keys.CurrentPlayersCountKey, FieldValue.Increment(1));
            bool addedOnce = false;
            for (int i = 0; i < docSnap.Get<int>(Keys.MaxPlayersCountKey) && !addedOnce; i++)
                if (docSnap.Get<string>(Keys.PlayerKey + i) == string.Empty)
                {
                    addedOnce = true;
                    await docRef.UpdateAsync(Keys.PlayerKey + i, leavingUserID);
                }
        }
        public override async Task DeleteGameFromDB(string id)
        {
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(id);
            await docRef.DeleteAsync();
        }
        public override async void GetPlayersFromDocument(string gameID,
            Action<ObservableCollection<User>> onCompleteChange)
        {
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            IDocumentSnapshot docSnap = await docRef.GetAsync();
            ObservableCollection<User> newList = [];
            
            for (int i = 0; i < docSnap.Get<int>(Keys.MaxPlayersCountKey); i++)
            {
                if (!(docSnap.Get<string>(Keys.PlayerKey + i) == null ||
                    docSnap.Get<string>(Keys.PlayerKey + i) == string.Empty))
                {
                    User tempUser = await UserIDToObject(docSnap.Get<string>(Keys.PlayerKey + i)!);
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
        private async Task<User> UserIDToObject(string id)
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

        public async void SetGameIsFull(string gameID)
        {
            IDocumentReference documentReference = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            await documentReference.UpdateAsync(Keys.IsFullKey, true);
        }

        public override async Task AddShape(Shape currentShape, string gameId)
        {
            IDocumentReference docRef = fs.Collection(Keys.GamesCollectionName).Document(gameId);
            await docRef.UpdateAsync(Keys.CurrentShapeIdKey, currentShape.Id);
            await docRef.UpdateAsync(Keys.CurrentShapeColorKey, currentShape.Color);
        }

        public IListenerRegistration? AddGameBoardListener(string gameID,
            Plugin.CloudFirestore.DocumentSnapshotHandler OnChange)
        {
            IDocumentReference dr = fs.Collection(Keys.GamesCollectionName).Document(gameID);
            return dr.AddSnapshotListener(OnChange);
        }
    }
}
