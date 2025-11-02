using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    internal class MainPageVM : ObservableObject, IQueryAttributable
    {
        public ICommand NavToLoginCommand { get => new Command(NavToLogin); }
        public ICommand NavToPlayCommand { get => new Command(NavToPlay); }
        public ICommand SignOutCommand { get => new Command(SignOut); }
        private readonly App? app;
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
            app = Application.Current as App;
            user = app!.user;
            Preferences.Clear();
            user.SignOut();
            RefreshProperties();
        }
        private void SeveralPropertiesChange()
        {
            string[] nameOfs = { nameof(LoginVisibility), nameof(SignOutVisibility), nameof(PlayVisibility) };
            for (int i = 0; i < nameOfs.Length; i++)
                OnPropertyChanged(nameOfs[i]);
        }
        private void RefreshProperties()
        {
            WelcomeUserName = $"{Strings.Welcome} {Preferences.Get(Keys.UserNameKey, TechnicalConsts.DefaultUserName)}{TechnicalConsts.ExclamationSign}";
            IsLogged = Preferences.Get(Keys.EmailKey, string.Empty) != string.Empty;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // This will be triggered every time the page is navigated to
            RefreshProperties();
        }
        private async void NavToLogin()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectLoginPageRefresh);
        }
        private async void NavToPlay()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectPlayPageRefresh);
        }
        private void SignOut()
        {
            user.SignOut();
            app!.user = new();
            RefreshProperties();
        }
    }
}
