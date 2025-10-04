using System.Xml.Linq;
using Tetris.Models;
using System.Text.Json;
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
        public override async Task Register()
        {
            await fbd.CreateUserAsync(Email, Password, UserName, OnCompleteRegister);
        }


        private void OnCompleteRegister(Task task)
        {
            if (task.IsCompletedSuccessfully)
            {
                SaveToPreferences();
            }
            else
            {
                Exception? ex = task.Exception?.InnerException;
                string errorMessage = string.Empty;

                if (ex != null)
                {
                    try
                    {
                        // Find the "Response:" part
                        int responseIndex = ex.Message.IndexOf("Response:");
                        if (responseIndex >= 0)
                        {
                            // Take everything after "Response:"
                            string jsonPart = ex.Message.Substring(responseIndex + "Response:".Length).Trim();

                            // Some Firebase responses might have extra closing braces, remove trailing stuff
                            int lastBrace = jsonPart.LastIndexOf('}');
                            if (lastBrace >= 0)
                                jsonPart = jsonPart.Substring(0, lastBrace + 1);

                            // Parse JSON
                            JsonDocument json = JsonDocument.Parse(jsonPart);

                            JsonElement errorElem = json.RootElement.GetProperty("error");
                            string firebaseMessage = errorElem.GetProperty("message").ToString();

                            switch (firebaseMessage)
                            {
                                case Keys.EmailExistsErrorKey:
                                    errorMessage = Strings.EmailExistsError;
                                    break;
                                case Keys.OperationNotAllowedErrorKey:
                                    errorMessage = Strings.OperationNotAllowedError;
                                    break;
                                case Keys.WeakPasswordErrorKey:
                                    errorMessage = Strings.WeakPasswordError;
                                    break;
                                case Keys.MissingEmailErrorKey:
                                    errorMessage = Strings.MissingEmailError;
                                    break;
                                case Keys.MissingPasswordErrorKey:
                                    errorMessage = Strings.MissingPasswordError;
                                    break;
                                case Keys.InvalidEmailErrorKey:
                                    errorMessage = Strings.InvalidEmailError;
                                    break;
                                default:
                                    errorMessage = Strings.DefaultRegisterError;
                                    break;
                            } 
                        }
                    }
                    catch
                    {
                        errorMessage = Strings.FailedJsonError;
                    }
                }
                Shell.Current.DisplayAlert(Strings.RegisterErrorTitle, errorMessage, Strings.Understood);
            }
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
