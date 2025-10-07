using System.Windows.Input;
using Tetris.ModelsLogic;
using Tetris.Models;

namespace Tetris.ViewModels
{
    internal class MainPageVM : ObservableObject
    {
        public ICommand NavToLoginCommand { get => new Command(NavToLogin); }
        public string WelcomeUserName { get; set; }
        readonly User user = new();
        public MainPageVM()
        {
            WelcomeUserName = Strings.Welcome + Preferences.Get(Keys.UserNameKey,"Guest");
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("refresh"))
            {
                WelcomeUserName = Strings.Welcome + Preferences.Get(Keys.UserNameKey, "Guest");
            }
        }

        private async void NavToLogin()
        {
            await Shell.Current.GoToAsync("///LoginPage");
        }
    }
}
