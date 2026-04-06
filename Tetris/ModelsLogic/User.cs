using CommunityToolkit.Maui.Alerts;
using Tetris.Interfaces;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    /// <summary>
    /// Represents a user in the Tetris application.
    /// Provides authentication, registration, password reset, and local preferences management.
    /// </summary>
    public class User : UserModel, IUser
    {
        #region Public Methods

        /// <summary>
        /// Attempts to log in the user using <see cref="Email"/> and Password.
        /// </summary>
        /// <returns>
        /// A Task{bool} indicating whether the login was successful.
        /// </returns>
        public override async Task<bool> Login()
        {
            bool success = await fbd.SignInWithEmailAndPWAsync(Email, Password, OnCompleteLogin);
            return success;
        }

        /// <summary>
        /// Registers a new user with <see cref="Email"/>, Password, and UserName.
        /// </summary>
        /// <returns>
        /// A Task{bool} indicating whether the registration was successful.
        /// </returns>
        public override async Task<bool> Register()
        {
            bool success = await fbd.CreateUserWithEmailAndPWAsync(Email, Password, UserName, OnCompleteRegister);
            return success;
        }

        /// <summary>
        /// Signs out the current user, clears local preferences, and resets the user object.
        /// </summary>
        public override void SignOut()
        {
            fbd.SignOut();
            Preferences.Clear();
            Reset();
        }

        /// <summary>
        /// Sends a password reset email to the user.
        /// </summary>
        public override async Task ResetPassword()
        {
            await fbd.SendPasswordResetEmailAsync(Email, OnCompleteSendEmail);
        }

        /// <summary>
        /// Validates if the user can log in with the current <see cref="Email"/> and Password.
        /// </summary>
        /// <returns>True if both email and password are valid; otherwise false.</returns>
        public override bool CanLogin()
        {
            return IsEmailValid() && IsPasswordValid();
        }

        /// <summary>
        /// Validates if the user can register using the current fields.
        /// </summary>
        /// <param name="repeatPassword">The repeated password to confirm match with Password.</param>
        /// <returns>True if all fields are valid and passwords match; otherwise false.</returns>
        public override bool CanRegister(string repeatPassword)
        {
            return IsUserNameValid() && IsPasswordValid() && IsEmailValid() && repeatPassword == Password;
        }

        /// <summary>
        /// Loads user data from local preferences.
        /// </summary>
        public override void Reset()
        {
            UserID = Preferences.Get(Keys.UserIDKey, string.Empty);
            UserName = Preferences.Get(Keys.UserNameKey, string.Empty);
            TotalLines = Preferences.Get(Keys.TotalLinesKey, 0);
            GamesPlayed = Preferences.Get(Keys.GamesPlayedKey, 0);
            HighestScore = Preferences.Get(Keys.HighestScoreKey, 0);
            DateJoined = Preferences.Get(Keys.DateJoinedKey,
                DateTime.Now.ToString(TechnicalConsts.DateFormat));
            Email = Preferences.Get(Keys.EmailKey, string.Empty);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handles the completion of the login task.
        /// On success, loads user data into preferences and shows a success toast.
        /// On failure, displays a login error alert.
        /// </summary>
        /// <param name="task">The login <see cref="Task"/>.</param>
        /// <returns>True if login succeeded; otherwise false.</returns>
        protected override async Task<bool> OnCompleteLogin(Task task)
        {
            bool result;

            if (task.IsCompletedSuccessfully)
            {
                await LoginSaveToPreferencesAsync();
                _ = Toast.Make(Strings.LoginSuccess, CommunityToolkit.Maui.Core.ToastDuration.Short, ConstData.ToastFontSize).Show();
                result = true;
            }
            else
            {
                string errorMessage = fbd.IdentifyFireBaseError(task);
                _ = Shell.Current.DisplayAlert(Strings.LoginErrorTitle, errorMessage, Strings.LoginFailButton);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Saves the current user's data retrieved from Firebase to local preferences.
        /// </summary>
        protected override async Task LoginSaveToPreferencesAsync()
        {
            UserID = fbd.GetCurrentUserID();
            UserName = await fbd.GetUserDataAsync<string>(Keys.UserNameKey);
            TotalLines = await fbd.GetUserDataAsync<int>(Keys.TotalLinesKey);
            GamesPlayed = await fbd.GetUserDataAsync<int>(Keys.GamesPlayedKey);
            HighestScore = await fbd.GetUserDataAsync<int>(Keys.HighestScoreKey);
            DateJoined = await fbd.GetUserDataAsync<string>(Keys.DateJoinedKey);

            SaveToPreferences();
        }

        /// <summary>
        /// Handles completion of the registration task.
        /// On success, saves user data and displays a success toast.
        /// On failure, displays a registration error alert.
        /// </summary>
        /// <param name="task">The registration <see cref="Task"/>.</param>
        /// <returns>True if registration succeeded; otherwise false.</returns>
        protected override bool OnCompleteRegister(Task task)
        {
            bool result;

            if (task.IsCompletedSuccessfully)
            {
                UserID = fbd.GetCurrentUserID();
                SaveToPreferences();
                _ = Toast.Make(Strings.RegisterSuccess, CommunityToolkit
                    .Maui.Core.ToastDuration.Short, ConstData.ToastFontSize).Show();
                result = true;
            }
            else
            {
                string errorMessage = fbd.IdentifyFireBaseError(task);
                _ = Shell.Current.DisplayAlert(Strings.RegisterErrorTitle,
                    errorMessage, Strings.RegisterFailButton);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Saves the current user data to local preferences.
        /// </summary>
        protected override void SaveToPreferences()
        {
            Preferences.Set(Keys.UserIDKey, UserID);
            Preferences.Set(Keys.UserNameKey, UserName);
            Preferences.Set(Keys.EmailKey, Email);
            Preferences.Set(Keys.TotalLinesKey, TotalLines);
            Preferences.Set(Keys.GamesPlayedKey, GamesPlayed);
            Preferences.Set(Keys.HighestScoreKey, HighestScore);
            Preferences.Set(Keys.DateJoinedKey, DateJoined);
        }

        /// <summary>
        /// Handles completion of sending a password reset email.
        /// Displays a success or error alert depending on task result.
        /// </summary>
        /// <param name="task">The password reset <see cref="Task"/>.</param>
        protected override void OnCompleteSendEmail(Task task)
        {
            if (task.IsCompletedSuccessfully)
                _ = Shell.Current.DisplayAlert(Strings.ResetPWTitle, 
                    Strings.ResetPWMessage, Strings.ResetPWButton);
            else
            {
                string errorMessage = fbd.IdentifyFireBaseError(task);
                _ = Shell.Current.DisplayAlert(Strings.ResetPWErrorTitle, 
                    errorMessage, Strings.ResetPWErrorButton);
            }
        }

        /// <summary>
        /// Validates the user's email address.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        protected override bool IsEmailValid()
        {
            bool result = true;

            if (Email.Length < ConstData.MinCharacterInEmail)
            {
                Shell.Current.DisplayAlert(
                    Strings.EmailShortErrorTitle,
                    dynamicStrings.EmailShortErrorMessage,
                    Strings.EmailShortErrorButton
                );
                result = false;
            }
            else if (!HasAtSign(Email) || !HasDot(Email))
            {
                Shell.Current.DisplayAlert(
                    Strings.EmailInvalidErrorTitle,
                    Strings.EmailInvalidErrorMessage,
                    Strings.EmailInvalidErrorButton
                );
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Validates the user's password.
        /// Must meet length and character requirements.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        protected override bool IsPasswordValid()
        {
            bool result = true;

            if (Password.Length < ConstData.MinCharacterInPW)
            {
                Shell.Current.DisplayAlert(
                    Strings.PasswordShortErrorTitle,
                    dynamicStrings.PasswordShortErrorMessage,
                    Strings.PasswordShortErrorButton
                );
                result = false;
            }
            else if (!HasNumber(Password))
            {
                Shell.Current.DisplayAlert(
                    Strings.PasswordNumberErrorTitle,
                    Strings.PasswordNumberErrorMessage,
                    Strings.PasswordNumberErrorButton
                );
                result = false;
            }
            else if (!HasLowerCase(Password))
            {
                Shell.Current.DisplayAlert(
                    Strings.PasswordLowerCaseErrorTitle,
                    Strings.PasswordLowerCaseErrorMessage,
                    Strings.PasswordLowerCaseErrorButton
                );
                result = false;
            }
            else if (!HasUpperCase(Password))
            {
                Shell.Current.DisplayAlert(
                    Strings.PasswordUpperCaseErrorTitle,
                    Strings.PasswordUpperCaseErrorMessage,
                    Strings.PasswordUpperCaseErrorButton
                );
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Validates the user's username.
        /// Must meet length and contain a number.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        protected override bool IsUserNameValid()
        {
            bool result = true;

            if (UserName.Length < ConstData.MinCharacterInUN)
            {
                Shell.Current.DisplayAlert(
                    Strings.UserNameShortErrorTitle,
                    dynamicStrings.UserNameShortErrorMessage,
                    Strings.UserNameShortErrorButton
                );
                result = false;
            }
            else if (!HasNumber(UserName))
            {
                Shell.Current.DisplayAlert(
                    Strings.UserNameNumberErrorTitle,
                    Strings.UserNameNumberErrorMessage,
                    Strings.UserNameNumberErrorButton
                );
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Checks if a string contains an '@' character.
        /// </summary>
        protected override bool HasAtSign(string str)
        {
            bool result = false;

            for (int i = 0; i < str.Length && !result; i++)
                if (str[i] == TechnicalConsts.AtSign)
                    result = true;

            return result;
        }

        /// <summary>
        /// Checks if a string contains a '.' character.
        /// </summary>
        protected override bool HasDot(string str)
        {
            bool result = false;

            for (int i = 0; i < str.Length && !result; i++)
                if (str[i] == TechnicalConsts.DotSign)
                    result = true;

            return result;
        }

        /// <summary>
        /// Checks if a string contains a numeric digit.
        /// </summary>
        protected override bool HasNumber(string str)
        {
            bool result = false;

            for (int i = 0; i < str.Length && !result; i++)
                if (str[i] >= TechnicalConsts.ZeroSign && str[i] <= TechnicalConsts.NineSign)
                    result = true;

            return result;
        }

        /// <summary>
        /// Checks if a string contains a lowercase letter.
        /// </summary>
        protected override bool HasLowerCase(string str)
        {
            bool result = false;

            for (int i = 0; i < str.Length && !result; i++)
                if (str[i] >= TechnicalConsts.ASign && str[i] <= TechnicalConsts.ZSign)
                    result = true;

            return result;
        }

        /// <summary>
        /// Checks if a string contains an uppercase letter.
        /// </summary>
        protected override bool HasUpperCase(string str)
        {
            bool result = false;

            for (int i = 0; i < str.Length && !result; i++)
                if (str[i] >= TechnicalConsts.CapitalASign && str[i] <= TechnicalConsts.CapitalZSign)
                    result = true;

            return result;
        }

        #endregion
    }
}