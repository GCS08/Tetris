using Microsoft.Extensions.Logging;
using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    internal class LoginPageVM : ObservableObject, IQueryAttributable
    {
        public ICommand NavToRegisterCommand => new Command(NavToRegister);
        public ICommand LoginCommand { get; }
        public ICommand ToggleIsPasswordCommand { get; }
        private readonly App? app;
        private readonly User user;
        public bool IsBusy { get; set; } = false;
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
            app = Application.Current as App;
            user = app!.user;
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
            bool successfullyLogged = await user.Login();
            IsBusy = false;
            OnPropertyChanged(nameof(IsBusy));
            if (successfullyLogged)
                await Shell.Current.GoToAsync("///MainPage?refresh=true");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // This will be triggered every time the page is navigated to
            RefreshProperties();
        }
        private void RefreshProperties()
        {
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(Password));
            Shell.Current.DisplayAlert(Strings.RegisterSuccessTitle, Email, Strings.RegisterSuccessButton);
            Shell.Current.DisplayAlert(Strings.RegisterSuccessTitle, Password, Strings.RegisterSuccessButton);
        }
        private async void NavToRegister()
        {
            await Shell.Current.GoToAsync("///RegisterPage");
        }
    }
}
