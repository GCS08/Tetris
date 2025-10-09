using System.Xml.Linq;
using Tetris.Models;
using System.Text.Json;
using static Tetris.Models.ConstData;
using System.Threading.Tasks;

namespace Tetris.ModelsLogic
{
    public class User : UserModel
    {
        private string IdentifyFireBaseError(Task task)
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

                        errorMessage = firebaseMessage switch
                        {
                            Keys.EmailExistsErrorKey => Strings.EmailExistsError,
                            Keys.OperationNotAllowedErrorKey => Strings.OperationNotAllowedError,
                            Keys.WeakPasswordErrorKey => Strings.WeakPasswordError,
                            Keys.MissingEmailErrorKey => Strings.MissingEmailError,
                            Keys.MissingPasswordErrorKey => Strings.MissingPasswordError,
                            Keys.InvalidEmailErrorKey => Strings.InvalidEmailError,
                            Keys.InvalidCredentialsErrorKey => Strings.InvalidCredentialsError,
                            Keys.UserDisabledErrorKey => Strings.UserDisabledError,
                            Keys.ManyAttemptsErrorKey => Strings.ManyAttemptsError,
                            _ => Strings.DefaultRegisterError,
                        };
                    }
                }
                catch
                {
                    errorMessage = Strings.FailedJsonError;
                }
            }
            return errorMessage;
        }
        //public override async Task<bool> LoginWithGoogle()
        //{
        //    try
        //    {
        //        // Replace with your Firebase auto-created Android client ID
        //        string clientId = "YOUR_ANDROID_CLIENT_ID.apps.googleusercontent.com";
        //        string redirectUri = "com.meitar.tetris:/oauth2redirect"; // your app scheme

        //        string state = Guid.NewGuid().ToString("N");
        //        string nonce = Guid.NewGuid().ToString("N");

        //        string authUrl =
        //            $"https://accounts.google.com/o/oauth2/v2/auth" +
        //            $"?client_id={clientId}" +
        //            $"&redirect_uri={redirectUri}" +
        //            $"&response_type=token%20id_token" +
        //            $"&scope=openid%20email%20profile" +
        //            $"&state={state}" +
        //            $"&nonce={nonce}";

        //        var result = await WebAuthenticator.Default.AuthenticateAsync(
        //            new Uri(authUrl),
        //            new Uri(redirectUri)
        //        );

        //        // id_token is returned in the Properties dictionary
        //        string idToken = result.Properties["id_token"];

        //        // Now send this to Firebase
        //        await fbd.SignInWithFirebaseGoogleIdToken(idToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        await Shell.Current.DisplayAlert("Error", $"Google Sign-In failed: {ex.Message}", "OK");
        //    }
        //}
        public override async Task<bool> Login()
        {
            bool success = await fbd.SignInWithEmailAndPWdAsync(Email, Password, OnCompleteLogin);
            return success;
        }
        private async Task<bool> OnCompleteLogin(Task task)
        {
            if (task.IsCompletedSuccessfully)
            {
                await LoginSaveToPreferencesAsync();
                await Shell.Current.DisplayAlert(Strings.LoginSuccessTitle, Strings.LoginSuccess, Strings.LoginSuccessButton);
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Login failed: {task.Exception?.InnerException?.Message}");
                string errorMessage = IdentifyFireBaseError(task);
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
                await Shell.Current.DisplayAlert(Strings.RegisterSuccessTitle, Strings.RegisterSuccess, Strings.RegisterSuccessButton);
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

        public override bool CanLogin()
        {
            return IsEmailValid() && IsPasswordValid();
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
