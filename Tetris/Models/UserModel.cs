using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class UserModel()
    {
        protected FbData fbd = new();
        public string UserID { get; set; } = Preferences.Get(Keys.UserIDKey, string.Empty);
        public string UserName { get; set; } = Preferences.Get(Keys.UserNameKey, string.Empty);
        public int TotalLines { get; set; } = Preferences.Get(Keys.TotalLinesKey, 0);
        public string TotalLinesDisplay =>
            Strings.TotalLinesShort + TotalLines.ToString();
        public int GamesPlayed { get; set; } = Preferences.Get(Keys.GamesPlayedKey, 0);
        public int HighestScore { get; set; } = Preferences.Get(Keys.HighestScoreKey, 0);
        public string HighestScoreDisplay =>
            Strings.HighestScoreShort + HighestScore.ToString();
        public bool Settings0 { get; set; } = Preferences.Get(Keys.Settings0Key, true);
        public bool Settings1 { get; set; } = Preferences.Get(Keys.Settings1Key, true);
        public bool Settings2 { get; set; } = Preferences.Get(Keys.Settings2Key, true);
        public string DateJoined { get; set; } = Preferences.Get(Keys.DateJoinedKey,
            DateTime.Now.ToString(TechnicalConsts.DateFormat));
        public string Email { get; set; } = Preferences.Get(Keys.EmailKey, string.Empty);
        public string Password { get; set; } = Preferences.Get(Keys.PasswordKey, string.Empty);
        protected readonly Strings dynamicStrings = new();
        public abstract Task Login();
        public abstract Task Register();
        public abstract void SignOut();
        public abstract bool CanLogin();
        public abstract bool CanRegister(string repeatPassword);
    }
}
