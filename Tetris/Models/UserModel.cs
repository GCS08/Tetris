using Tetris.Interfaces;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    /// <summary>
    /// Represents an abstract user model providing properties for user information and methods for authentication,
    /// registration, and validation.
    /// </summary>
    public abstract class UserModel()
    {
        #region Fields
        protected FbData fbd = IPlatformApplication.Current?.Services.GetService<IFbData>() as FbData ?? new();
        protected readonly Strings dynamicStrings = new();
        #endregion

        #region Properties
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
        public string Password { get; set; } = string.Empty;
        #endregion

        #region Public Methods
        public abstract Task<bool> Login();
        public abstract Task<bool> Register();
        public abstract void SignOut();
        public abstract Task ResetPassword();
        public abstract bool CanLogin();
        public abstract bool CanRegister(string repeatPassword);
        public abstract void Reset();
        #endregion

        #region Protected Methods
        protected abstract Task<bool> OnCompleteLogin(Task task);
        protected abstract Task LoginSaveToPreferencesAsync();
        protected abstract bool OnCompleteRegister(Task task);
        protected abstract void SaveToPreferences();
        protected abstract void OnCompleteSendEmail(Task task);
        protected abstract bool IsEmailValid();
        protected abstract bool IsPasswordValid();
        protected abstract bool IsUserNameValid();
        protected abstract bool HasAtSign(string str);
        protected abstract bool HasDot(string str);
        protected abstract bool HasNumber(string str);
        protected abstract bool HasLowerCase(string str);
        protected abstract bool HasUpperCase(string str);
        #endregion
    }
}
