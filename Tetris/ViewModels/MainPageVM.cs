using System.Windows.Input;
using Tetris.ModelsLogic;
using Tetris.Models;

namespace Tetris.ViewModels
{
    internal class MainPageVM : ObservableObject
    {
        public ICommand NavToLoginCommand { get => new Command(NavToLogin); }
        public User user { get; set; }
        public string WelcomeUserName { get; set; }
        public MainPageVM()
        {
            user = new User();
            WelcomeUserName = Strings.Welcome + user.UserName;
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // This is called EVERY TIME navigation lands on this page
            string name = Preferences.Get(Keys.UserNameKey, "Guest");
            WelcomeUserName = Strings.Welcome + name;
            OnPropertyChanged(WelcomeUserName);
        }
        private async void NavToLogin()
        {
            await Shell.Current.GoToAsync("///LoginPage");
        }
    }
}
