using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class UserModel
    {
        protected FbData fbd = new();
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public abstract Task Login();
        public abstract Task Register();
        public abstract void SignOut();
        public abstract bool CanLogin();
        public abstract bool CanRegister(string repeatPassword);
    }
}
