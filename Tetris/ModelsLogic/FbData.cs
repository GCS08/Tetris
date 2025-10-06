using Firebase.Auth;
using Firebase.Auth.Providers;
using Plugin.CloudFirestore;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    class FbData:FbDataModel
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
        class FbDataAboutUser : FbData
        {
            public async Task<T?> GetUserFieldAsync<T>(string key)
            {
                try
                {
                    var snapshot = await fdb.Collection("users").Document(facl.User.Uid).GetAsync();

                    if (!snapshot.Exists)
                        return default; // document does not exist

                    var data = snapshot.Data; // this is Dictionary<string, object>
                    if (data == null || !data.ContainsKey(key))
                        return default; // field does not exist

                    object value = data[key]!; // get the object

                    if (value is T tValue)
                        return tValue;

                    return (T?)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return default; // return null/default if anything fails
                }
            }
        }
    }
}
