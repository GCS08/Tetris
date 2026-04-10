using Firebase.Auth;
using Firebase.Auth.Providers;
using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    /// <summary>
    /// Provides an abstract base class for managing Firebase authentication and Firestore operations, including user
    /// management, game data handling, and real-time updates.
    /// </summary>
    public abstract class FbDataModel
    {
        #region Fields
        protected FirebaseAuthClient facl;
        protected IFirestore fs;
        #endregion

        #region Constructors
        public FbDataModel()
        {
            FirebaseAuthConfig fac = new()
            {
                ApiKey = Keys.FbApiKey,
                AuthDomain = Keys.FbAppDomainKey,
                Providers = [new EmailProvider()]
            };
            facl = new FirebaseAuthClient(fac);
            fs = CrossCloudFirestore.Current.Instance;
        }
        #endregion

        #region Public Methods
        public abstract Task<bool> CreateUserWithEmailAndPWAsync(string email
            , string password, string userName, Func<Task, bool> OnCompleteRegister);
        public abstract Task<bool> SignInWithEmailAndPWAsync(string email, 
            string password, Func<Task, Task<bool>> OnCompleteLogin);
        public abstract void SignOut();
        public abstract string GetCurrentUserID();
        public abstract Task SendPasswordResetEmailAsync(string email, 
            Action<Task> OnCompleteSendEmail);
        public abstract Task<T> GetUserDataAsync<T>(string key);
        public abstract string IdentifyFireBaseError(Task task);
        public abstract string AddGameToDB(string userID, string creatorName, string cubeColor,
            int currentPlayersCount, int maxPlayersCount, bool isFull, int firstShapeId,
            int firstShapeInGameId, string firstShapeColor, List<Shape> firstShapes, bool isPublicGame);
        public abstract IListenerRegistration AddGamesCollectionListener(
            Plugin.CloudFirestore.QuerySnapshotHandler OnChange);
        public abstract void GetAvailGames(Action<ObservableCollection<Game>> onComplete);
        public abstract Task<ObservableCollection<Game>> GetAvailGamesList();
        public abstract Task OnPlayerLeaveWR(string id, string leavingUserID);
        public abstract Task OnPlayerJoinWR(string id, string leavingUserID);
        public abstract void DeleteGameFromDB(string id);
        public abstract void GetPlayersFromDocument(string gameID,
            Action<ObservableCollection<ModelsLogic.User>> onCompleteChange);
        public abstract Task<int> GetCurrentPlayersCount(string gameID);
        public abstract void SetGameIsFull(string gameID);
        public abstract void AddShape(Shape currentShape, string gameId);
        public abstract Shape CreateShape(IDocumentSnapshot snapshot);
        public abstract Task UploadMoves(string userID, string gameID,
            ModelsLogic.Queue<string> movesQueue);
        public abstract IListenerRegistration? AddGameListener(string gameID,
        Plugin.CloudFirestore.DocumentSnapshotHandler OnChange);
        public abstract void SetPlayerReady(string gameID, 
            int maxPlayersCount, string userID);
        public abstract void ResetIsShapeAtBottom(string gameID, int desiredIndex);
        public abstract Task DeleteFbDocsAsync();
        public abstract void UpdateUserPostGame(ModelsLogic.User user);
        public abstract Task<Game> GetGameByCode(int code);
        public abstract Task<bool> SetPrivateJoinCode(string gameID, int code);
        #endregion

        #region Protected Methods
        protected abstract Task<ModelsLogic.User> UserIDToObject(string id);
        #endregion
    }
}
