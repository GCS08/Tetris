using System.Windows.Input;
using Tetris.ModelsLogic;
using Tetris.Models;

namespace Tetris.ViewModels
{
    internal class MainPageVM : ObservableObject, IQueryAttributable
    {
        public ICommand NavToLoginCommand { get => new Command(NavToLogin); }
        private bool isRegistered;
        public bool LoginVisibility
        {
            get => !isRegistered;
            set
            {
                if (isRegistered != value)
                {
                    isRegistered = value;
                    OnPropertyChanged(nameof(LoginVisibility));
                }
            }
        }
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
            Preferences.Clear();
            RefreshProperties();
        }

        private void RefreshProperties()
        {
            WelcomeUserName = $"{Strings.Welcome} {Preferences.Get(Keys.UserNameKey, "Guest")}!";
            LoginVisibility = Preferences.Get(Keys.EmailKey, string.Empty) != string.Empty;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // This will be triggered every time the page is navigated to
            RefreshProperties();
        }

        private async void NavToLogin()
        {
            await Shell.Current.GoToAsync("///LoginPage");
        }
    }
}
