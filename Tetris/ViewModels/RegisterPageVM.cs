using CommunityToolkit.Maui.Alerts;
using System.Windows.Input;
using Tetris.Interfaces;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    /// <summary>
    /// ViewModel for the Register page.
    /// Handles user registration, random username generation, password visibility toggling, and navigation.
    /// </summary>
    public partial class RegisterPageVM : ObservableObject
    {
        #region Fields
        private readonly User User = IPlatformApplication.Current?.
            Services.GetService<IUser>() as User ?? new();
        private string passwordRepeat;
        #endregion

        #region ICommands
        public ICommand NavToLoginCommand => new Command(NavToLogin);
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand RandomUsernameCommand => new Command(GetRandomUsername);
        public ICommand RegisterCommand { get; }
        public ICommand ToggleIsPassword1Command { get; }
        public ICommand ToggleIsPassword2Command { get; }
        #endregion

        #region Properties
        public bool RegisterEnable { get; set; } = true;
        public bool IsBusy { get; set; } = false;

        public string UserName
        {
            get => User?.UserName ?? Strings.UsernameUa;
            set
            {
                if (User != null && User.UserName != value)
                    User.UserName = value;
            }
        }

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

        public string PasswordRepeat
        {
            get => passwordRepeat;
            set
            {
                if (value != passwordRepeat)
                    passwordRepeat = value;
            }
        }

        public bool IsPassword1 { get; set; } = true;
        public bool IsPassword2 { get; set; } = true;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="RegisterPageVM"/>.
        /// Sets up the commands and refreshes properties.
        /// </summary>
        public RegisterPageVM()
        {
            RefreshProperties();
            passwordRepeat = string.Empty;
            RegisterCommand = new Command(async () => await Register());
            ToggleIsPassword1Command = new Command(ToggleIsPassword1);
            ToggleIsPassword2Command = new Command(ToggleIsPassword2);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Toggles the visibility of the first password field.
        /// </summary>
        private void ToggleIsPassword1()
        {
            IsPassword1 = !IsPassword1;
            OnPropertyChanged(nameof(IsPassword1));
        }

        /// <summary>
        /// Toggles the visibility of the repeat password field.
        /// </summary>
        private void ToggleIsPassword2()
        {
            IsPassword2 = !IsPassword2;
            OnPropertyChanged(nameof(IsPassword2));
        }

        /// <summary>
        /// Determines whether registration can be performed.
        /// </summary>
        /// <returns>True if valid, false otherwise.</returns>
        private bool CanRegister()
        {
            return User.CanRegister(passwordRepeat);
        }

        /// <summary>
        /// Performs the registration process asynchronously and navigates to the main page on success.
        /// </summary>
        private async Task Register()
        {
            if (CanRegister())
            {
                IsBusy = true;
                OnPropertyChanged(nameof(IsBusy));
                RegisterEnable = false;
                OnPropertyChanged(nameof(RegisterEnable));

                bool successfullyRegistered = await User.Register();
                if (successfullyRegistered)
                    _ = Shell.Current.Navigation.PushAsync(new MainPage());

                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
                RegisterEnable = true;
                OnPropertyChanged(nameof(RegisterEnable));
            }
        }

        /// <summary>
        /// Generates a random username asynchronously and updates the UserName property.
        /// </summary>
        private async void GetRandomUsername()
        {
            RandomUsername randomUsername = new();
            UserName = await randomUsername.GetAsync();
            OnPropertyChanged(nameof(UserName));
        }

        /// <summary>
        /// Refreshes the password fields and raises property changed notifications for all relevant properties.
        /// </summary>
        private void RefreshProperties()
        {
            IsPassword1 = true;
            IsPassword2 = true;
            SeveralPropertiesChange();
        }

        /// <summary>
        /// Raises property changed events for all major fields to refresh the UI.
        /// </summary>
        private void SeveralPropertiesChange()
        {
            string[] nameOfs = [nameof(UserName), nameof(Email),
                nameof(Password), nameof(IsPassword1), nameof(IsPassword2)];
            for (int i = 0; i < nameOfs.Length; i++)
                OnPropertyChanged(nameOfs[i]);
        }

        /// <summary>
        /// Navigates to the login page.
        /// </summary>
        private void NavToLogin()
        {
            Shell.Current.Navigation.PushAsync(new LoginPage());
        }

        /// <summary>
        /// Navigates back to the main page.
        /// </summary>
        private void NavHome()
        {
            User.Reset();
            Shell.Current.Navigation.PushAsync(new MainPage());
        }

        #endregion
    }
}