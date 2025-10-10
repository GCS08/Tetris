using Firebase.Auth;
using Firebase.Auth.Providers;
using Plugin.CloudFirestore;
namespace Tetris.Models
{
    public abstract class FbDataModel
    {
        protected FirebaseAuthClient facl;
        protected IFirestore fdb;
        public abstract Task<bool> CreateUserWithEmailAndPWAsync(string email, string password, string userName, Func<Task, Task<bool>> OnCompleteRegister);
        public abstract Task<bool> SignInWithEmailAndPWdAsync(string email, string password, Func<Task, Task<bool>> OnCompleteLogin);
        public abstract void SignOut();
        public abstract Task<T> GetUserDataAsync<T>(string key);
        // 1) FbDataModel constructor — make sure GoogleProvider is included
        public FbDataModel()
        {
            FirebaseAuthConfig fac = new()
            {
                ApiKey = Keys.FbApiKey,
                AuthDomain = "tetris-gcs71.firebaseapp.com",
                Providers =
                [
                    new EmailProvider(),
                    new GoogleProvider().AddScopes("email", "profile") // <-- add Google provider
                ]
                // optionally set UserRepository if you want persistence
            };
            facl = new FirebaseAuthClient(fac);
            fdb = CrossCloudFirestore.Current.Instance;
        }


    }
}
