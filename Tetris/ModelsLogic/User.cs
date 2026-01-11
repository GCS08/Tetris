using CommunityToolkit.Maui.Alerts;
using Tetris.Models;
using static Tetris.Models.ConstData;

namespace Tetris.ModelsLogic
{
    public class User : UserModel
    {        
        public override async Task<bool> Login()// cannot be sync because of firestore method
        {
            bool success = await fbd.SignInWithEmailAndPWAsync(Email, Password, OnCompleteLogin);
            return success;
        }
        protected override async Task<bool> OnCompleteLogin(Task task)// cannot be sync because of firestore method
        {
            if (task.IsCompletedSuccessfully)
            {
                await LoginSaveToPreferencesAsync();
                _ = Toast.Make(Strings.LoginSuccess, CommunityToolkit.Maui.Core.ToastDuration.Short, ConstData.ToastFontSize).Show();
                return true;
            }
            else
            {
                string errorMessage = fbd.IdentifyFireBaseError(task);
                _ = Shell.Current.DisplayAlert(Strings.LoginErrorTitle, errorMessage, Strings.LoginFailButton);
                return false;
            }
        }
        protected override async Task LoginSaveToPreferencesAsync()// cannot be sync because of firestore method
        {
            // Await all async Firebase reads
            UserID = fbd.GetCurrentUserID();
            UserName = await fbd.GetUserDataAsync<string>(Keys.UserNameKey);
            TotalLines = await fbd.GetUserDataAsync<int>(Keys.TotalLinesKey);
            GamesPlayed = await fbd.GetUserDataAsync<int>(Keys.GamesPlayedKey);
            HighestScore = await fbd.GetUserDataAsync<int>(Keys.HighestScoreKey);
            Settings0 = await fbd.GetUserDataAsync<bool>(Keys.Settings0Key);
            Settings1 = await fbd.GetUserDataAsync<bool>(Keys.Settings1Key);
            Settings2 = await fbd.GetUserDataAsync<bool>(Keys.Settings2Key);
            DateJoined = await fbd.GetUserDataAsync<string>(Keys.DateJoinedKey);

            SaveToPreferences();
        }
        public override async Task<bool> Register()// cannot be sync because of firestore method
        {
            bool success = await fbd.CreateUserWithEmailAndPWAsync(Email, Password, UserName, OnCompleteRegister);
            return success;
        }
        protected override bool OnCompleteRegister(Task task)
        {
            if (task.IsCompletedSuccessfully)
            {
                UserID = fbd.GetCurrentUserID();
                SaveToPreferences();
                _ = Toast.Make(Strings.RegisterSuccess,
                    CommunityToolkit.Maui.Core.ToastDuration.Short,
                    ConstData.ToastFontSize).Show();
                return true;
            }
            else
            {
                string errorMessage = fbd.IdentifyFireBaseError(task);
                _ = Shell.Current.DisplayAlert(Strings.RegisterErrorTitle,
                    errorMessage, Strings.RegisterFailButton);
                return false;
            }
        }
        protected override void SaveToPreferences()
        {
            // Now store everything in Preferences
            Preferences.Set(Keys.UserIDKey, UserID);
            Preferences.Set(Keys.UserNameKey, UserName);
            Preferences.Set(Keys.EmailKey, Email);
            Preferences.Set(Keys.PasswordKey, Password);
            Preferences.Set(Keys.TotalLinesKey, TotalLines);
            Preferences.Set(Keys.GamesPlayedKey, GamesPlayed);
            Preferences.Set(Keys.HighestScoreKey, HighestScore);
            Preferences.Set(Keys.Settings0Key, Settings0);
            Preferences.Set(Keys.Settings1Key, Settings1);
            Preferences.Set(Keys.Settings2Key, Settings2);
            Preferences.Set(Keys.DateJoinedKey, DateJoined);
        }
        public override void SignOut()
        {
            fbd.SignOut();
            Preferences.Clear();
        }
        public override async Task ResetPassword() // cannot be sync because of firestore method
        {
            await fbd.SendPasswordResetEmailAsync(Email, OnCompleteSendEmail);
        }

        protected override void OnCompleteSendEmail(Task task)
        {
            if (task.IsCompletedSuccessfully)
            {
                _ = Shell.Current.DisplayAlert(Strings.ResetPWTitle, Strings.ResetPWMessage, Strings.ResetPWButton);
            }
            else
            {
                string errorMessage = fbd.IdentifyFireBaseError(task);
                _ = Shell.Current.DisplayAlert(Strings.ResetPWErrorTitle, errorMessage, Strings.ResetPWErrorButton);
            }
        }
        public override bool CanLogin()
        {
            return IsEmailValid() && IsPasswordValid();
        }
        public override bool CanRegister(string repeatPassword)
        {
            return IsUserNameValid() && IsPasswordValid() && IsEmailValid() && repeatPassword == Password;
        }
        protected override bool IsEmailValid()
        {
            if (Email.Length < MinCharacterInEmail)
            {
                Shell.Current.DisplayAlert(Strings.EmailShortErrorTitle, dynamicStrings.EmailShortErrorMessage, Strings.EmailShortErrorButton);
                return false;
            }
            if (!HasAtSign(Email) || !HasDot(Email))
            {
                Shell.Current.DisplayAlert(Strings.EmailInvalidErrorTitle, Strings.EmailInvalidErrorMessage, Strings.EmailInvalidErrorButton);
                return false;
            }
            return true;
        }
        protected override bool IsPasswordValid()
        {
            if (Password.Length < MinCharacterInPW)
            {
                Shell.Current.DisplayAlert(Strings.PasswordShortErrorTitle, dynamicStrings.PasswordShortErrorMessage, Strings.PasswordShortErrorButton);
                return false;
            }
            if (!HasNumber(Password))
            {
                Shell.Current.DisplayAlert(Strings.PasswordNumberErrorTitle, Strings.PasswordNumberErrorMessage, Strings.PasswordNumberErrorButton);
                return false;
            }
            if (!HasLowerCase(Password))
            {
                Shell.Current.DisplayAlert(Strings.PasswordLowerCaseErrorTitle, Strings.PasswordLowerCaseErrorMessage, Strings.PasswordLowerCaseErrorButton);
                return false;
            }
            if (!HasUpperCase(Password))
            {
                Shell.Current.DisplayAlert(Strings.PasswordUpperCaseErrorTitle, Strings.PasswordUpperCaseErrorMessage, Strings.PasswordUpperCaseErrorButton);
                return false;
            }
            return true;
        }
        protected override bool IsUserNameValid()
        {
            if (UserName.Length < MinCharacterInUN)
            {
                Shell.Current.DisplayAlert(Strings.UserNameShortErrorTitle, dynamicStrings.UserNameShortErrorMessage, Strings.UserNameShortErrorButton);
                return false;
            }
            if (!HasNumber(UserName))
            {
                Shell.Current.DisplayAlert(Strings.UserNameNumberErrorTitle, Strings.UserNameNumberErrorMessage, Strings.UserNameNumberErrorButton);
                return false;
            }
            return true;
        }
        protected override bool HasAtSign(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] == TechnicalConsts.AtSign)
                    return true;
            return false;
        }
        protected override bool HasDot(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] == TechnicalConsts.DotSign)
                    return true;
            return false;
        }
        protected override bool HasNumber(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] >= TechnicalConsts.ZeroSign && str[i] <= TechnicalConsts.NineSign)
                    return true;
            return false;
        }
        protected override bool HasLowerCase(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] >= TechnicalConsts.ASign && str[i] <= TechnicalConsts.ZSign)
                    return true;
            return false;
        }
        protected override bool HasUpperCase(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] >= TechnicalConsts.CapitalASign && str[i] <= TechnicalConsts.CapitalZSign)
                    return true;
            return false;
        }
    }
}
