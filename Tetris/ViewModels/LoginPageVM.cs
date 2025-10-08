using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    internal class LoginPageVM : ObservableObject
    {
        public ICommand NavToRegisterCommand => new Command(NavToRegister);
        public ICommand LoginCommand { get; }
        public ICommand ToggleIsPasswordCommand { get; }
        public bool IsBusy { get; set; } = false;
        private User user = new();
        public string Email
        {
            get => user.Email;
            set
            {
                user.Email = value;
                (LoginCommand as Command)?.ChangeCanExecute();
            }
        }
        public string Password
        {
            get => user.Password;
            set
            {
                user.Password = value;
                (LoginCommand as Command)?.ChangeCanExecute();
            }
        }
        public bool IsPassword { get; set; } = true;
        public LoginPageVM()
        {
            LoginCommand = new Command(async () => await Login(), CanLogin);
            ToggleIsPasswordCommand = new Command(ToggleIsPassword);
        }
        private void ToggleIsPassword()
        {
            IsPassword = !IsPassword;
            OnPropertyChanged(nameof(IsPassword));
        }
        private bool CanLogin()
        {
            return user.CanLogin();
        }
        private async Task Login()
        {
            IsBusy = true;
            OnPropertyChanged(nameof(IsBusy));
            await user.Login();
            IsBusy = false;
            OnPropertyChanged(nameof(IsBusy));
            await Shell.Current.GoToAsync("///MainPage?refresh=true");

        }
        private async void NavToRegister()
        {
            await Shell.Current.GoToAsync("///RegisterPage");
        }
    }
}
