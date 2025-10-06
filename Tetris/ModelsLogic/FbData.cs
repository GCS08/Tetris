using Firebase.Auth;
using Firebase.Auth.Providers;
using Plugin.CloudFirestore;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    class FbData:FbDataModel
    {
        public override async Task CreateUserAsync(string email, string password, string userName, Action<Task> OnCompleteRegister)
        {
            Task<Firebase.Auth.UserCredential> task = facl.CreateUserWithEmailAndPasswordAsync(email, password, userName);

            try
            {
                // Try creating the user
                UserCredential credential = await task;
                Firebase.Auth.User user = facl.User;

                await fdb.Collection("users").Document(user.Uid).SetAsync(new
                {
                    UserName = userName,
                    Email = email,
                    Password = password,
                    DateJoined = DateTime.Now.ToString("dd/MM/yy"),
                    GamesPlayed = 0,
                    HighestScore = 0,
                    Settings0 = true,
                    Settings1 = true,
                    Settings2 = true,
                    TotalLinesCleared = 0,
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


        public override async void SignInWithEmailAndPasswordAsync(string email, string password, Action<System.Threading.Tasks.Task> OnComplete)
        {
            await facl.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(OnComplete);
        }

        public override string DisplayName
        {
            get
            {
                string dn = string.Empty;
                if (facl.User != null)
                    dn = facl.User.Info.DisplayName;
                return dn;
            }
        }
        public override string UserId
        {
            get
            {
                return facl.User.Uid;
            }
        }
    }
}
