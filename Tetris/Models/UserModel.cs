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
        public string DateJoined { get; set; } = Preferences.Get(Keys.DateJoinedKey,
            DateTime.Now.ToString(TechnicalConsts.DateFormat));
        public string Email { get; set; } = Preferences.Get(Keys.EmailKey, string.Empty);
        public string Password { get; set; } = Preferences.Get(Keys.PasswordKey, string.Empty);
        protected readonly Strings dynamicStrings = new();

        public abstract Task<bool> Login();
        protected abstract Task<bool> OnCompleteLogin(Task task);
        protected abstract Task LoginSaveToPreferencesAsync();
        public abstract Task<bool> Register();
        protected abstract bool OnCompleteRegister(Task task);
        protected abstract void SaveToPreferences();
        public abstract void SignOut();
        public abstract Task ResetPassword();
        protected abstract void OnCompleteSendEmail(Task task);
        public abstract bool CanLogin();
        public abstract bool CanRegister(string repeatPassword);
        protected abstract bool IsEmailValid();
        protected abstract bool IsPasswordValid();
        protected abstract bool IsUserNameValid();
        protected abstract bool HasAtSign(string str);
        protected abstract bool HasDot(string str);
        protected abstract bool HasNumber(string str);
        protected abstract bool HasLowerCase(string str);
        protected abstract bool HasUpperCase(string str);

    }
}
