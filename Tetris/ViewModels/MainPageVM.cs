using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    public partial class MainPageVM : ObservableObject
    {
        public ICommand NavToLoginCommand { get => new Command(NavToLogin); }
        public ICommand NavToGameLobbyCommand { get => new Command(NavToGameLobby); }
        public ICommand SignOutCommand { get => new Command(SignOut); }
        private User user;
        private bool isLogged;
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
        public bool LoginVisibility => !IsLogged;
        private string? welcomeUserName;
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

        public MainPageVM()
        {
            user = (Application.Current as App)!.AppUser;
            //SignOut();
            RefreshProperties();
        }
        private void SeveralPropertiesChange()
        {
            string[] nameOfs = [nameof(LoginVisibility), nameof(SignOutVisibility), nameof(PlayVisibility)];
            for (int i = 0; i < nameOfs.Length; i++)
                OnPropertyChanged(nameOfs[i]);
        }
        private void RefreshProperties()
        {
            if (user.UserName != string.Empty)
                WelcomeUserName = Strings.Welcome + TechnicalConsts.SpaceSign + 
                    user.UserName + TechnicalConsts.ExclamationSign;
            else
                WelcomeUserName = Strings.Welcome + TechnicalConsts.SpaceSign +
                    TechnicalConsts.DefaultUserName + TechnicalConsts.ExclamationSign;
            IsLogged = user.UserID != string.Empty;
        }
        private void NavToLogin()
        {
            Shell.Current.Navigation.PushAsync(new LoginPage());
        }
        private void NavToGameLobby()
        {
            Tetris.Platforms.Android.SoundManager.Instance.PlayLineCleared();
            _ = Shell.Current.Navigation.PushAsync(new GameLobbyPage());
        }
        private void SignOut()
        {
            user.SignOut();
            (Application.Current as App)!.AppUser = new();
            user = (Application.Current as App)!.AppUser;
            RefreshProperties();
        }
    }
}
