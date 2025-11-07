using Microsoft.Extensions.Logging;
using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    public class LoginPageVM : ObservableObject, IQueryAttributable
    {
        public ICommand NavToRegisterCommand => new Command(NavToRegister);
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand ToggleIsPasswordCommand { get; }
        public bool LoginEnable { get; set; } = true;
        private App? app;
        private User user;
        public bool IsBusy { get; set; } = false;
        public string Email
        {
            get => user.Email;
            set
            {
                user.Email = value;
            }
        }
        public string Password
        {
            get => user.Password;
            set
            {
                user.Password = value;
            }
        }
        public bool IsPassword { get; set; } = true;
        public LoginPageVM()
        {
            app = Application.Current as App;
            user = app!.user;
            LoginCommand = new Command(async () => await Login());
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
            if (CanLogin())
            {
                IsBusy = true;
                OnPropertyChanged(nameof(IsBusy));
                LoginEnable = false;
                OnPropertyChanged(nameof(LoginEnable));
                bool successfullyLogged = await user.Login();
                if (successfullyLogged)
                    await Shell.Current.GoToAsync(TechnicalConsts.RedirectMainPageRefresh);
                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
                LoginEnable = true;
                OnPropertyChanged(nameof(LoginEnable));
            }
        }
        private async Task ForgotPassword()
        {
            IsBusy = true;
            OnPropertyChanged(nameof(IsBusy));
            await user.ResetPassword();
            IsBusy = false;
            OnPropertyChanged(nameof(IsBusy));
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
            string[] nameOfs = [nameof(Email), nameof(Password), nameof(IsPassword)];
            for (int i = 0; i < nameOfs.Length; i++)
                OnPropertyChanged(nameOfs[i]);
        }
        private async void NavToRegister()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectRegisterPageRefresh);
        }
        private async void NavHome()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectMainPageRefresh);
        }
    }
}
