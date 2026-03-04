using System.Windows.Input;
using Tetris.Interfaces;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels
{
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
        public MainPageVM()
        {
            //SignOut();
            RefreshProperties();
        }
        #endregion

        #region Private Methods
        private void SeveralPropertiesChange()
        {
            string[] nameOfs = [nameof(LoginVisibility), nameof(RemindersVisibility)
                , nameof(SignOutVisibility), nameof(PlayVisibility)];
            for (int i = 0; i < nameOfs.Length; i++)
                OnPropertyChanged(nameOfs[i]);
        }
    
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
     
        private void NavToReminders()
        {
            Shell.Current.Navigation.PushAsync(new RemindersPage());
        }
      
        private void NavToLogin()
        {
            Shell.Current.Navigation.PushAsync(new LoginPage());
        }
      
        private void NavToGameLobby()
        {
            Shell.Current.Navigation.PushAsync(new GameLobbyPage());
        }
    
        private void SignOut()
        {
            if (User == null) return;
            User.SignOut();
            RefreshProperties();
        }
        #endregion
    }
}
