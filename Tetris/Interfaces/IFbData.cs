using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using Tetris.ModelsLogic;

namespace Tetris.Interfaces
{
    public interface IFbData
    {
        public Task<bool> CreateUserWithEmailAndPWAsync(string email, string password, string userName, Func<Task, bool> OnCompleteRegister);
        public Task<bool> SignInWithEmailAndPWAsync(string email, string password, Func<Task, Task<bool>> OnCompleteLogin);
        public void SignOut();
        public string GetCurrentUserID();
        public Task SendPasswordResetEmailAsync(string email, Action<Task> OnCompleteSendEmail);
        public Task<T> GetUserDataAsync<T>(string key);
        public string IdentifyFireBaseError(Task task);
        public string AddGameToDB(string userID, string creatorName, string cubeColor,
            int currentPlayersCount, int maxPlayersCount, bool isFull, int currentShapeId,
            int currentShapeInGameId, string currentShapeColor, bool isPublicGame);
        public IListenerRegistration AddGamesCollectionListener(
            Plugin.CloudFirestore.QuerySnapshotHandler OnChange);
        public void GetAvailGames(Action<ObservableCollection<Game>> onComplete);
        public Task<ObservableCollection<Game>> GetAvailGamesList();
        public Task OnPlayerLeaveWR(string id, string leavingUserID);
        public Task OnPlayerJoinWR(string id, string leavingUserID);
        public void DeleteGameFromDB(string id);
        public void GetPlayersFromDocument(string gameID,
            Action<ObservableCollection<ModelsLogic.User>> onCompleteChange);
        public Task<int> GetCurrentPlayersCount(string gameID);
        public void SetGameIsFull(string gameID);
        public void AddShape(Shape currentShape, string gameId);
        public Shape CreateShape(IDocumentSnapshot snapshot);
        public Task FinishRound(string userID, string gameID, ModelsLogic.Queue<string> movesQueue);
        public IListenerRegistration? AddGameListener(string gameID,
        Plugin.CloudFirestore.DocumentSnapshotHandler OnChange);
        public void SetPlayerReady(string gameID, int maxPlayersCount, string userID);
        public void ResetIsShapeAtBottom(string gameID, int desiredIndex);
        public void DeleteFbDocs();
        public void UpdateUserPostGame(ModelsLogic.User user);
        public Task<Game> GetGameByCode(int code);
        public Task<bool> SetPrivateJoinCode(string gameID, int code);
    }
}
