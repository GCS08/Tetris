using System.Windows.Input;
using Tetris.Interfaces;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    /// <summary>
    /// ViewModel for the Main Page.
    /// Manages user state, navigation, and UI visibility based on login status.
    /// </summary>
    public partial class MainPageVM : ObservableObject
    {
        #region Fields

        private string? welcomeUserName;
        private bool isLogged;
        public User User = IPlatformApplication.Current?.Services.GetService<IUser>() as User ?? new();
        #endregion

        #region ICommands
        public ICommand NavToLoginCommand { get => new Command(NavToLogin); }
        public ICommand NavToGameLobbyCommand { get => new Command(NavToGameLobby); }
        public ICommand NavToRemindersCommand { get => new Command(NavToReminders); }
        public ICommand SignOutCommand { get => new Command(SignOut); }
        #endregion

        #region Properties
        private bool IsLogged
        {
            get => isLogged;
            set
            {
                if (isLogged != value)
                {
                    isLogged = value;
                    SeveralPropertiesChange();
                }
            }
        }

        public bool SignOutVisibility => IsLogged;
        public bool PlayVisibility => IsLogged;
        public bool RemindersVisibility => IsLogged;
        public bool LoginVisibility => !IsLogged;

        public string? WelcomeUserName
        {
            get => welcomeUserName;
            set
            {
                if (welcomeUserName != value)
                {
                    welcomeUserName = value;
                    OnPropertyChanged(nameof(WelcomeUserName));
                }
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="MainPageVM"/>.
        /// Refreshes bound properties based on the current user state.
        /// </summary>
        public MainPageVM()
        {
            RefreshProperties();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Triggers OnPropertyChanged for multiple UI visibility properties.
        /// </summary>
        private void SeveralPropertiesChange()
        {
            string[] nameOfs = [nameof(LoginVisibility), nameof(RemindersVisibility)
                , nameof(SignOutVisibility), nameof(PlayVisibility)];
            for (int i = 0; i < nameOfs.Length; i++)
                OnPropertyChanged(nameOfs[i]);
        }

        /// <summary>
        /// Refreshes the UI-bound properties like welcome message and visibility based on user login state.
        /// </summary>
        private void RefreshProperties()
        {
            if (User != null && User.UserName != string.Empty)
                WelcomeUserName = Strings.Welcome + TechnicalConsts.SpaceSign +
                    User.UserName + TechnicalConsts.ExclamationSign;
            else
                WelcomeUserName = Strings.Welcome + TechnicalConsts.SpaceSign +
                    TechnicalConsts.DefaultUserName + TechnicalConsts.ExclamationSign;

            IsLogged = User != null && User.UserID != string.Empty;
        }

        /// <summary>
        /// Navigates to the Reminders page.
        /// </summary>
        private void NavToReminders()
        {
            Shell.Current.Navigation.PushAsync(new RemindersPage());
        }

        /// <summary>
        /// Navigates to the Login page.
        /// </summary>
        private void NavToLogin()
        {
            Shell.Current.Navigation.PushAsync(new LoginPage());
        }

        /// <summary>
        /// Navigates to the Game Lobby page.
        /// </summary>
        private void NavToGameLobby()
        {
            Shell.Current.Navigation.PushAsync(new GameLobbyPage());
        }

        /// <summary>
        /// Signs out the current user and refreshes UI properties.
        /// </summary>
        private void SignOut()
        {
            if (User == null) return;
            User.SignOut();
            RefreshProperties();
        }

        #endregion
    }
}