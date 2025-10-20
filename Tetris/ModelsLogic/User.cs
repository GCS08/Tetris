using CommunityToolkit.Maui.Alerts;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tetris.Models;
using static Tetris.Models.ConstData;

namespace Tetris.ModelsLogic
{
    public class User : UserModel
    {
        readonly Strings dynamicStrings = new();
        
        public override async Task<bool> Login()
        {
            bool success = await fbd.SignInWithEmailAndPWAsync(Email, Password, OnCompleteLogin);
            return success;
        }
        public async Task<bool> LoginWithGoogle()
        {
            bool success = await fbd.SignInWithGoogleAsync(Email, Password, OnCompleteLogin);
            return success;
        }
        private async Task<bool> OnCompleteLogin(Task task)
        {
            if (task.IsCompletedSuccessfully)
            {
                await LoginSaveToPreferencesAsync();
                await Toast.Make(Strings.LoginSuccess, CommunityToolkit.Maui.Core.ToastDuration.Short, ConstData.ToastFontSize).Show();
                return true;
            }
            else
            {
                string errorMessage = fbd.IdentifyFireBaseError(task);
                await Shell.Current.DisplayAlert(Strings.LoginErrorTitle, errorMessage, Strings.LoginFailButton);
                return false;
            }
        }
        private async Task LoginSaveToPreferencesAsync()
        {
            // Await all async Firebase reads
            string userName = await fbd.GetUserDataAsync<string>(Keys.UserNameKey);
            int totalLines = await fbd.GetUserDataAsync<int>(Keys.TotalLinesKey);
            int gamesPlayed = await fbd.GetUserDataAsync<int>(Keys.GamesPlayedKey);
            int highestScore = await fbd.GetUserDataAsync<int>(Keys.HighestScoreKey);
            bool settings0 = await fbd.GetUserDataAsync<bool>(Keys.Settings0Key);
            bool settings1 = await fbd.GetUserDataAsync<bool>(Keys.Settings1Key);
            bool settings2 = await fbd.GetUserDataAsync<bool>(Keys.Settings2Key);
            string dateJoined = await fbd.GetUserDataAsync<string>(Keys.DateJoinedKey);

            // Now store everything in Preferences
            Preferences.Set(Keys.UserNameKey, userName);
            Preferences.Set(Keys.EmailKey, Email);
            Preferences.Set(Keys.PasswordKey, Password);
            Preferences.Set(Keys.TotalLinesKey, totalLines);
            Preferences.Set(Keys.GamesPlayedKey, gamesPlayed);
            Preferences.Set(Keys.HighestScoreKey, highestScore);
            Preferences.Set(Keys.Settings0Key, settings0);
            Preferences.Set(Keys.Settings1Key, settings1);
            Preferences.Set(Keys.Settings2Key, settings2);
            Preferences.Set(Keys.DateJoinedKey, dateJoined);
        }
        public override async Task<bool> Register()
        {
            bool success = await fbd.CreateUserWithEmailAndPWAsync(Email, Password, UserName, OnCompleteRegister);
            return success;
        }
        private async Task<bool> OnCompleteRegister(Task task)
        {
            if (task.IsCompletedSuccessfully)
            {
                RegisterSaveToPreferences();
                await Toast.Make(Strings.RegisterSuccess, CommunityToolkit.Maui.Core.ToastDuration.Short, ConstData.ToastFontSize).Show();
                return true;
            }
            else
            {
                string errorMessage = IdentifyFireBaseError(task);
                await Shell.Current.DisplayAlert(Strings.RegisterErrorTitle, errorMessage, Strings.RegisterFailButton);
                return false;
            }
        }
        private void RegisterSaveToPreferences()
        {
            Preferences.Set(Keys.UserNameKey, UserName);
            Preferences.Set(Keys.EmailKey, Email);
            Preferences.Set(Keys.PasswordKey, Password);
            Preferences.Set(Keys.TotalLinesKey, 0);
            Preferences.Set(Keys.GamesPlayedKey, 0);
            Preferences.Set(Keys.HighestScoreKey, 0);
            Preferences.Set(Keys.Settings0Key, true);
            Preferences.Set(Keys.Settings1Key, true);
            Preferences.Set(Keys.Settings2Key, true);
            Preferences.Set(Keys.DateJoinedKey, DateTime.Now.ToString("dd/MM/yy"));
        }
        public override void SignOut()
        {
            fbd.SignOut();
            Preferences.Clear();
        }
        public async Task ResetPassword()
        {
            await fbd.SendPasswordResetEmailAsync(Email, OnCompleteSendEmail);
        }
        private async Task OnCompleteSendEmail(Task task)
        {
            if (task.IsCompletedSuccessfully)
            {
                await Shell.Current.DisplayAlert(Strings.ResetPWTitle, Strings.ResetPWMessage, Strings.ResetPWButton);
            }
            else
            {
                string errorMessage = IdentifyFireBaseError(task);
                await Shell.Current.DisplayAlert(Strings.ResetPWErrorTitle, errorMessage, Strings.ResetPWErrorButton);
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
        private bool IsEmailValid()
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
        private bool IsPasswordValid()
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
        private bool IsUserNameValid()
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
