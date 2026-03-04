using System.Windows.Input;
using Tetris.Interfaces;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    /// <summary>
    /// ViewModel for the Login page.
    /// Handles login, password reset, navigation, and password visibility toggling.
    /// Binds the <see cref="User"/> model to the UI.
    /// </summary>
    public partial class LoginPageVM : ObservableObject
    {
        #region Fields

        public User User = IPlatformApplication.Current?.
            Services.GetService<IUser>() as User ?? new();
        #endregion

        #region ICommands
        public ICommand NavToRegisterCommand => new Command(NavToRegister);
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand ToggleIsPasswordCommand { get; }
        #endregion

        #region Properties
        public bool LoginEnable { get; set; } = true;
        public bool IsBusy { get; set; } = false;
        public string Email
        {
            get => User?.Email ?? Strings.EmailUa;
            set
            {
                if (User != null && User.Email != value)
                    User.Email = value;
            }
        }
        public string Password
        {
            get => User?.Password ?? Strings.PasswordUa;
            set
            {
                if (User != null && User.Password != value)
                    User.Password = value;
            }
        }
        public bool IsPassword { get; set; } = true;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="LoginPageVM"/>.
        /// Sets up commands and refreshes bound properties.
        /// </summary>
        public LoginPageVM()
        {
            RefreshProperties();
            LoginCommand = new Command(async () => await Login());
            ForgotPasswordCommand = new Command(async () => await ForgotPassword());
            ToggleIsPasswordCommand = new Command(ToggleIsPassword);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Toggles the password visibility on the UI.
        /// </summary>
        private void ToggleIsPassword()
        {
            IsPassword = !IsPassword;
            OnPropertyChanged(nameof(IsPassword));
        }

        /// <summary>
        /// Checks if login is possible based on current user state and validation.
        /// </summary>
        /// <returns>True if login is allowed, otherwise false.</returns>
        private bool CanLogin()
        {
            if (User == null) return false;
            return User.CanLogin();
        }

        /// <summary>
        /// Attempts to log the user in asynchronously.
        /// Updates UI busy state and navigates to <see cref="MainPage"/> on success.
        /// </summary>
        private async Task Login()
        {
            if (User != null && CanLogin())
            {
                IsBusy = true;
                OnPropertyChanged(nameof(IsBusy));
                LoginEnable = false;
                OnPropertyChanged(nameof(LoginEnable));

                bool successfullyLogged = await User.Login();
                if (successfullyLogged)
                    _ = Shell.Current.Navigation.PushAsync(new MainPage());

                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
                LoginEnable = true;
                OnPropertyChanged(nameof(LoginEnable));
            }
        }

        /// <summary>
        /// Sends a password reset request asynchronously.
        /// Updates UI busy state during the operation.
        /// </summary>
        private async Task ForgotPassword()
        {
            if (User == null) return;
            IsBusy = true;
            OnPropertyChanged(nameof(IsBusy));

            await User.ResetPassword();

            IsBusy = false;
            OnPropertyChanged(nameof(IsBusy));
        }

        /// <summary>
        /// Resets all ViewModel properties to their initial state.
        /// </summary>
        private void RefreshProperties()
        {
            IsPassword = true;
            SeveralPropertiesChange();
        }

        /// <summary>
        /// Triggers OnPropertyChanged for multiple properties.
        /// </summary>
        private void SeveralPropertiesChange()
        {
            string[] nameOfs = [nameof(Email), nameof(Password), nameof(IsPassword)];
            for (int i = 0; i < nameOfs.Length; i++)
                OnPropertyChanged(nameOfs[i]);
        }

        /// <summary>
        /// Navigates to the registration page.
        /// </summary>
        private void NavToRegister()
        {
            Shell.Current.Navigation.PushAsync(new RegisterPage());
        }

        /// <summary>
        /// Navigates back to the main page/home.
        /// </summary>
        private void NavHome()
        {
            Shell.Current.Navigation.PushAsync(new MainPage());
        }

        #endregion
    }
}