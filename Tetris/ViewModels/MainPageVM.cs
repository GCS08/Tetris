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
            Preferences.Clear();
            WelcomeUserName = Strings.Welcome + Preferences.Get(Keys.UserNameKey,"Guest");
        }
        
        public void Refresh()
        {
            WelcomeUserName = Strings.Welcome + Preferences.Get(Keys.UserNameKey, "Guest");
            OnPropertyChanged(WelcomeUserName);
        }


        private async void NavToLogin()
        {
            await Shell.Current.GoToAsync("///LoginPage");
        }
    }
}
