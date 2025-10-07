using Firebase.Auth;
using Firebase.Auth.Providers;
using Plugin.CloudFirestore;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class FbData : FbDataModel
    {
        public override async Task CreateUserWithEmailAndPWAsync(string email, string password, string userName, Action<Task> OnCompleteRegister)
        {
            // Prepare the task
            Task<Firebase.Auth.UserCredential> task = facl.CreateUserWithEmailAndPasswordAsync(email, password, userName);

            try
            {
                // Register the user
                UserCredential credential = await task;
                Firebase.Auth.User user = credential.User;

                // Immediately sign in the new user to ensure request.auth is not null
                await facl.SignInWithEmailAndPasswordAsync(email, password);
                
                
                var userId = facl.User.Uid;
                // Now the user is authenticated, we can safely write to Firestore
                await fdb.Collection("users").Document(userId).SetAsync(new
                {
                    userName = userName,
                    email = email,
                    password = password,
                    dateJoined = DateTime.Now.ToString("dd/MM/yy"),
                    gamesPlayed = 0,
                    highestScore = 0,
                    settings0 = true,
                    settings1 = true,
                    settings2 = true,
                    totalLinesCleared = 0,
                });

            }
            catch (Exception ex)
            {
                // If the Firebase call failed, store the exception manually
                TaskCompletionSource<Firebase.Auth.UserCredential> tcs = new();
                tcs.SetException(ex);
                task = tcs.Task;
            }
            finally
            {
                // Always invoke the callback, even after a failure
                OnCompleteRegister(task);
            }
        }
        public override async Task SignInWithEmailAndPWdAsync(string email, string password, Action<System.Threading.Tasks.Task> OnCompleteLogin)
        {
            // Prepare the task
            Task<Firebase.Auth.UserCredential> task = facl.SignInWithEmailAndPasswordAsync(email, password);

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                // If the Firebase call failed, store the exception manually
                TaskCompletionSource<Firebase.Auth.UserCredential> tcs = new();
                tcs.SetException(ex);
                task = tcs.Task;
            }
            finally
            {
                // Always invoke the callback, even after a failure
                OnCompleteLogin(task);
            }
        }
        public override async Task<T> GetUserDataAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(facl.User?.Uid))
                return default!;

            IDocumentSnapshot? snapshot = await fdb.Collection("users").Document(facl.User.Uid).GetAsync();
            if (snapshot.Exists)
            {
                T? value = snapshot.Get<T>(key);
                return value != null ? value : default!;
            }
            return default!;
        }
        public override void SignOut()
        {
            try
            {
                await facl.SignOut();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(facl == null);
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
