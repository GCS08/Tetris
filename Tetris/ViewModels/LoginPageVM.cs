using Microsoft.Extensions.Logging;
using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    internal class LoginPageVM : ObservableObject, IQueryAttributable
    {
        public ICommand NavToRegisterCommand => new Command(NavToRegister);
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand ToggleIsPasswordCommand { get; }
        private App? app;
        private User user;
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
            ForgotPasswordCommand = new Command(async () => await ForgotPassword());
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
        private async Task ForgotPassword()
        {
            user.ResetPassword();
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            app = Application.Current as App;
            user = app!.user;
            RefreshProperties();
        }
        private void RefreshProperties()
        {
            IsPassword = true;
            SeveralPropertiesChange();
        }
        private void SeveralPropertiesChange()
        {
            string[] nameOfs = { nameof(Email), nameof(Password), nameof(IsPassword) };
            for (int i = 0; i < nameOfs.Length; i++)
                OnPropertyChanged(nameOfs[i]);
        }
        private async void NavToRegister()
        {
            await Shell.Current.GoToAsync("///RegisterPage?refresh=true");
        }
        private async void NavHome()
        {
            await Shell.Current.GoToAsync("///MainPage?refresh=true");
        }
    }
}
