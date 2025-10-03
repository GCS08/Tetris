using System.Xml.Linq;
using Tetris.Models;
using static Tetris.Models.ConstData;
    
namespace Tetris.ModelsLogic
{
    class User : UserModel
    {
        public User()
        {
            Preferences.Get(Keys.UserNameKey, "Guest");
            Preferences.Get(Keys.EmailKey, string.Empty);
            Preferences.Get(Keys.PasswordKey, string.Empty);
            Preferences.Get(Keys.TotalLinesKey, 0);
            Preferences.Get(Keys.GamesPlayedKey, 0);
            Preferences.Get(Keys.HighestScoreKey, 0);
            Preferences.Get(Keys.Settings0Key, true);
            Preferences.Get(Keys.Settings1Key, true);
            Preferences.Get(Keys.Settings2Key, true);
            Preferences.Get(Keys.DateJoinedKey, DateTime.Now.ToString("dd/MM/yy"));
        }
        public override bool Login() { return true; }
        public override void Register()
        {
            fbd.CreateUserAsync(UserName, Password, Email, OnCompleteRegister);
        }

        private void OnCompleteRegister(Task task)
        {
            if (task.IsCompletedSuccessfully)
                SaveToPreferences();
            else
                Shell.Current.DisplayAlert(Strings.CreatUserError, task.Exception?.Message, Strings.Ok);
        }

        private void SaveToPreferences()
        {
            Preferences.Set(Keys.UserNameKey, UserName);
            Preferences.Set(Keys.EmailKey, Email);
            Preferences.Set(Keys.PasswordKey, Password);
            Preferences.Set(Keys.TotalLinesKey, 0);
            Preferences.Set(Keys.GamesPlayedKey, 0);
            Preferences.Set(Keys.HighestScoreKey, 0);
            Preferences.Set(Keys.Settings0Key, 0);
            Preferences.Set(Keys.Settings1Key, 0);
            Preferences.Set(Keys.Settings2Key, 0);
            Preferences.Set(Keys.DateJoinedKey, 0);
        }

        public override bool CanLogin()
        {
            return IsUserNameValid() && IsPasswordValid();
        }
        public override bool CanRegister()
        {
            return IsUserNameValid() && IsPasswordValid() && IsEmailValid();
        }
        private bool IsEmailValid()
        {
            if (Email.Length < MinCharacterInEmail || !HasAtSign(Email)
                || !HasDot(Email))
                return false;
            return true;
        }
        private bool IsPasswordValid()
        {
            if (Password.Length < MinCharacterInPW || !HasNumber(Password)
                || !HasLowerCase(Password) || !HasUpperCase(Password))
                return false;
            return true;
        }
        private bool IsUserNameValid()
        {
            if (UserName.Length < MinCharacterInUN || !HasNumber(UserName))
                return false;
            return true;
        }
        private static bool HasAtSign(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] == '@')
                    return true;
            return false;
        }
        private static bool HasDot(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] == '.')
                    return true;
            return false;
        }
        private static bool HasNumber(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] >= '0' && str[i] <= '9')
                    return true;
            return false;
        }
        private static bool HasLowerCase(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] >= 'a' && str[i] <= 'z')
                    return true;
            return false;
        }
        private static bool HasUpperCase(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] >= 'A' && str[i] <= 'Z')
                    return true;
            return false;
        }
        
    }
}
