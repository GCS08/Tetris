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
            // Always build the task first
            Task task = facl.CreateUserWithEmailAndPasswordAsync(email, password, userName);

            // Run it and call the callback when done
            try
            {
                await task;
            }
            finally
            {
                // Invoke the callback no matter what happened
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
