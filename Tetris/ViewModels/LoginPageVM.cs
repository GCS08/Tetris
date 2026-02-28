using System.Windows.Input;
using Tetris.Interfaces;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    public partial class LoginPageVM : ObservableObject
    {
        public User User = IPlatformApplication.Current?.Services.GetService<IUser>() as User ?? new();
        public ICommand NavToRegisterCommand => new Command(NavToRegister);
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand ToggleIsPasswordCommand { get; }
        public bool LoginEnable { get; set; } = true;
        public bool IsBusy { get; set; } = false;
        public string Email
        {
            get => User?.Email ?? Strings.EmailUa;
            set
            {
                if (User != null && User.Email != value)
                    User.Email = value;
            }
        }
        public string Password
        {
            get => User?.Password ?? Strings.PasswordUa;
            set
            {
                if (User != null && User.Password != value)
                    User.Password = value;
            }
        }
        public bool IsPassword { get; set; } = true;
        public LoginPageVM()
        {
            RefreshProperties();
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
            if (User == null) return false;
            return User.CanLogin();
        }
        private async Task Login()
        {
            if (User != null && CanLogin())
            {
                IsBusy = true;
                OnPropertyChanged(nameof(IsBusy));
                LoginEnable = false;
                OnPropertyChanged(nameof(LoginEnable));
                bool successfullyLogged = await User.Login();
                if (successfullyLogged)
                    _ = Shell.Current.Navigation.PushAsync(new MainPage());
                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
                LoginEnable = true;
                OnPropertyChanged(nameof(LoginEnable));
            }
        }
        private async Task ForgotPassword()
        {
            if (User == null) return;
            IsBusy = true;
            OnPropertyChanged(nameof(IsBusy));
            await User.ResetPassword();
            IsBusy = false;
            OnPropertyChanged(nameof(IsBusy));
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
        private void NavToRegister()
        {
            Shell.Current.Navigation.PushAsync(new RegisterPage());
        }
        private void NavHome()
        {
            Shell.Current.Navigation.PushAsync(new MainPage());
        }
    }
}
