namespace Tetris.Models
{
    public class Strings
    {
        public const string UserName = "Enter your username:";
        public const string Email = "Enter your email:";
        public const string Password = "Enter your password:";
        public const string passwordRepeat = "Confirm your password:";
        public const string LoginTitle = "Login to Tetris!";
        public const string RegisterTitle = "Register to Tetris!";
        public const string SubmitLoginButtonText = "Login!";
        public const string SubmitRegisterButtonText = "Register!";
        public const string DontOwnAccountText = "Don't have an account? ";
        public const string OwnAccountText = "Already have an account? ";
        public const string Register = "Register!";
        public const string Login = "Login!";
        public const string LoginOrRegisterButton = "Login/Register? Click me!";
        public const string SignOutButton = "Sign out";
        public const string RemindersTitleText = "Set a Play Reminder!";
        public const string Welcome = "Welcome";
        public const string EmailExistsError = "The email address is already in use by another account.";
        public const string OperationNotAllowedError = "Unable to create an account in this method.";
        public const string WeakPasswordError = "The password is too weak. It should be at least 8 characters.";
        public const string MissingEmailError = "Please provide an email to create an account.";
        public const string MissingPasswordError = "Please provide a password to create an account.";
        public const string InvalidEmailError = "Please provide a valid email to create an account.";
        public const string InvalidCredentialsError = "One of the provided credentials (email/password) was incorrect.\n" +
            "Please re-check your input and try again.";
        public const string UserDisabledError = "Your account has been disabled.\n" +
            "Contact our team at 'shaysol1233@gmail.com' for more details.";
        public const string ManyAttemptsError = "There have been too many failed attempts, and your account has been temporarily disabled.\n" +
            "Please try again later.";
        public const string DefaultError = "Something went wrong, please try again later.\n" +
            "If the error persists, contact our developer team at 'shaysol1233@gmail.com'.";
        public const string FailedJsonError = "Something went wrong.\nThe system couldn't identify the error.\n" +
            "Please try again";
        public const string RegisterErrorTitle = "Error creating a user:";
        public const string LoginErrorTitle = "Error logging in:";
        public const string RegisterFailButton = "Understood!";
        public const string LoginFailButton = "I'll try again!";
        public const string RegisterSuccess = "Thank you for creating an account! Enjoy Tetris!\nA verification email was sent.";
        public const string LoginSuccess = "Welcome back! Enjoy our Tetris!";
        public const string LoginWithGoogleButtonText = "Login with Google!";
        public const string ForgotPassword = "Forgot your password? ";
        public const string ClickMe = "Click me!";
        public const string ResetPWTitle = "An email has been sent:";
        public const string ResetPWErrorTitle = "Error sending a mail:";
        public const string ResetPWMessage = "An email with a link to reset your password has been sent to the provided email.\n" +
            "Please follow the instructions in the email and try again.";
        public const string ResetPWButton = "I will!";
        public const string ResetPWErrorButton = "I'll right it right away!";
        public const string EmailShortErrorTitle = "Email too short:";
        public string EmailShortErrorMessage = "The email you provided is too short.\n" +
            "Your email's minimum length must be " + ConstData.MinCharacterInEmail + " characters.\n" +
            "Please re-check it and try again.";
        public const string EmailShortErrorButton = "e-kay@gmail.com!";
        public const string EmailInvalidErrorTitle = "Invalid email:";
        public const string EmailInvalidErrorMessage = "The email you provided is invalid.\n" +
            "Please make sure it has '@' sign and a '.' and try again.";
        public const string EmailInvalidErrorButton = "ok@y.";
        public const string PasswordShortErrorTitle = "Password too short:";
        public string PasswordShortErrorMessage = "The password you provided is too short.\n" +
            "Your password's minimum length must be " + ConstData.MinCharacterInPW + " characters.\n" +
            "Please re-check it and try again.";
        public const string PasswordShortErrorButton = "oki-doki!";
        public const string PasswordNumberErrorTitle = "Password doesn't have a number:";
        public const string PasswordNumberErrorMessage = "Your password must contain at least one number.\n" +
            "Please add one (or any other number) and try again.";
        public const string PasswordNumberErrorButton = "5ure!";
        public const string PasswordLowerCaseErrorTitle = "Password doesn't have a lower-case letter:";
        public const string PasswordLowerCaseErrorMessage = "Your password must contain at least one lower-case letter.\n" +
            "Please add one and try again.";
        public const string PasswordLowerCaseErrorButton = "SURE!";
        public const string PasswordUpperCaseErrorTitle = "Password doesn't have an upper-case letter:";
        public const string PasswordUpperCaseErrorMessage = "Your password must contain at least one upper-case letter.\n" +
            "Please add one and try again.";
        public const string PasswordUpperCaseErrorButton = "sure!";
        public const string UserNameShortErrorTitle = "Username too short:";
        public string UserNameShortErrorMessage = "The username you provided is too short.\n" +
            "Your username's minimum length must be " + ConstData.MinCharacterInUN + " characters.\n" +
            "Please re-check it and try again.";
        public const string NotificationSuccess = "Notification created successfully.";
        public const string NotificationFail = "Notification creation has failed.";
        public const string NotificationTitle = "Your Tetris misses you!";
        public const string NotificationContent = "Come on! Align those blocks before they stack to the top of the board :(";
        public const string FalseCode = "False code. Check your spell and try again.";
        public const string CodeInterview = "Your room code is:\n";
        public const string UserNameShortErrorButton = "FineThenOkay!";
        public const string UserNameNumberErrorTitle = "Username doesn't have a number:";
        public const string UserNameNumberErrorMessage = "Your username must contain at least one number.\n" +
            "Please add one (or any other number) and try again.";
        public const string UserNameNumberErrorButton = "0kay!";
        public const string EmailVerificationError = "Email not verified.\nPlease verify your email before logging in.";
        public const string PlayButtonText = "Play!";
        public const string PlaySoloButtonText = "Play solo";
        public const string CreateNewRoomButtonText = "Create new room";
        public const string AvailableGamesTitle = "Available Games:";
        public const string JoinGameButtonText = "Join!";
        public const string FailedRandomApiUN = "UnknownUser";
        public const string CreateNewGameTitle = "Create new game:";
        public const string CreateGameButtonText = "Create game!";
        public const string WaitingRoomTitle = "Waiting Room:";
        public const string PlayersInRoomTitle = "Players waiting:";
        public const string TotalLinesShort = "tlc: ";
        public const string HighestScoreShort = "hs: ";
        public const string FeelingReady = "Feeling ready???";
        public const string TimeUp = "May the best win!";
        public const string NotificationChannelDescription = "The default channel for notifications.";
        public const string AlarmReceiverBroadcastLabel = "Local Notifications Broadcast Receiver";
        public const string UaUsername = "Username's unavailable";
        public const string YouLost = "You lost.\nBetter luck next time!";
        public const string YouWon = "You won.\nGood job!";
        public const string LoadingResult = "Loading result...\nPlease wait.";
        public const string BackToGameLobby = "Back to game lobby";
        public const string RemindersButtonText = "Reminders";
    }
}
