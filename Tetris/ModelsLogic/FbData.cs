using Firebase.Auth;
using Firebase.Auth.Providers;
using Plugin.CloudFirestore;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    class FbData:FbDataModel
    {
        public override async void CreateUserAsync(string userName, string password, string email, Action<Task> OnCompleteRegister)
        {
            await facl.CreateUserWithEmailAndPasswordAsync(userName, password, email).ContinueWith(OnCompleteRegister);
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
