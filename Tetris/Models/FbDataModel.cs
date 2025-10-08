using Firebase.Auth;
using Firebase.Auth.Providers;
using Plugin.CloudFirestore;
namespace Tetris.Models
{
    public abstract class FbDataModel
    {
        protected FirebaseAuthClient facl;
        protected IFirestore fdb;
        public abstract Task CreateUserWithEmailAndPWAsync(string email, string password, string name, Action<System.Threading.Tasks.Task> OnComplete);
        public abstract Task<bool> SignInWithEmailAndPWdAsync(string email, string password, Func<Task, Task<bool>> OnCompleteLogin);
        public abstract void SignOut();
        public abstract Task<T> GetUserDataAsync<T>(string key);
        public FbDataModel()
        {
            FirebaseAuthConfig fac = new()
            {
                ApiKey = Keys.FbApiKey,
                AuthDomain = Keys.FbAppDomainKey,
                Providers = [new EmailProvider()]
            };
            facl = new FirebaseAuthClient(fac);
            fdb = CrossCloudFirestore.Current.Instance;
        }
    }
}
