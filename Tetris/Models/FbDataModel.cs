using Firebase.Auth;
using Firebase.Auth.Providers;
using Plugin.CloudFirestore;
namespace Tetris.Models
{
    public abstract class FbDataModel
    {
        protected FirebaseAuthClient facl;
        protected IFirestore fs;
        public abstract Task<bool> CreateUserWithEmailAndPWAsync(string email, string password, string userName, Func<Task, Task<bool>> OnCompleteRegister);
        public abstract Task<bool> SignInWithEmailAndPWAsync(string email, string password, Func<Task, Task<bool>> OnCompleteLogin);
        public abstract void SignOut();
        public abstract Task SendPasswordResetEmailAsync(string email, Func<Task, Task> OnCompleteSendEmail);
        public abstract Task<T> GetUserDataAsync<T>(string key);
        public abstract string IdentifyFireBaseError(Task task);
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
