using Firebase.Auth;
using Firebase.Auth.Providers;
using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class FbDataModel
    {
        protected FirebaseAuthClient facl;
        protected IFirestore fs;
        public abstract Task<bool> CreateUserWithEmailAndPWAsync(string email, string password, string userName, Func<Task, Task<bool>> OnCompleteRegister);
        public abstract Task<bool> SignInWithEmailAndPWAsync(string email, string password, Func<Task, Task<bool>> OnCompleteLogin);
        public abstract void SignOut();
        public abstract string GetCurrentUserID();
        public abstract Task SendPasswordResetEmailAsync(string email, Func<Task, Task> OnCompleteSendEmail);
        public abstract Task<T> GetUserDataAsync<T>(string key);
        public abstract string IdentifyFireBaseError(Task task);
        public abstract Task<string> AddGameToDB(string userID, string creatorName, string cubeColor,
            int currentPlayersCount, int maxPlayersCount, bool isFull, int currentShapeId,
            int currentShapeInGameId, string currentShapeColor, bool isPublicGame);
        public abstract IListenerRegistration AddGamesCollectionListener(
            Plugin.CloudFirestore.QuerySnapshotHandler OnChange);
        public abstract void GetAvailGames(Action<ObservableCollection<Game>> onComplete);
        public abstract Task<ObservableCollection<Game>> GetAvailGamesList();
        public abstract Task OnPlayerLeaveWR(string id, string leavingUserID);
        public abstract Task OnPlayerJoinWR(string id, string leavingUserID);
        public abstract Task DeleteGameFromDB(string id);
        public abstract void GetPlayersFromDocument(string gameID,
            Action<ObservableCollection<ModelsLogic.User>> onCompleteChange);
        public abstract Task<int> GetCurrentPlayersCount(string gameID);
        public abstract Task AddShape(Shape currentShape, string gameId);
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

    }
}
