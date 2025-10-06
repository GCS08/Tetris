using Tetris.ModelsLogic;

namespace Tetris.Models
{
    abstract class UserModel
    {
        protected FbData fbd = new();
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int TotalLines { get; set; } = 0;
        public int GamesPlayed { get; set; } = 0;
        public int HighestScore { get; set; } = 0;
        public bool Settings0 { get; set; } = true;
        public bool Settings1 { get; set; } = true;
        public bool Settings2 { get; set; } = true;
        public string DateJoined { get; set; } = string.Empty;
        public abstract Task Login();
        public abstract Task Register();
        public abstract bool CanLogin();
        public abstract bool CanRegister();
    }
}
