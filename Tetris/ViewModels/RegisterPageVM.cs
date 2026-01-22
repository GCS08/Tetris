using CommunityToolkit.Maui.Alerts;
using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    public class RegisterPageVM : ObservableObject
    {
        public ICommand NavToLoginCommand => new Command(NavToLogin);
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand RandomUsernameCommand => new Command(GetRandomUsername);
        public ICommand RegisterCommand { get; }
        public ICommand ToggleIsPassword1Command { get; }
        public ICommand ToggleIsPassword2Command { get; }
        public bool RegisterEnable { get; set; } = true;
        public bool IsBusy { get; set; } = false;
        private readonly User user;
        public string UserName
        {
            get => user.UserName;
            set
            {
                if (value != user.UserName)
                {
                    user.UserName = value;
                }
            }
        }
        public string Email
        {
            get => user.Email;
            set
            {
                if (value != user.Email)
                {
                    user.Email = value;
                }
            }
        }
        public string Password
        {
            get => user.Password;
            set
            {
                if (value != user.Password)
                {
                    user.Password = value;
                }
            }
        }
        private string passwordRepeat;
        public string PasswordRepeat
        {
            get => passwordRepeat;
            set
            {
                if (value != passwordRepeat)
                {
                    passwordRepeat = value;
                }
            }
        }
        public bool IsPassword1 { get; set; } = true;
        public bool IsPassword2 { get; set; } = true;
        public RegisterPageVM()
        {
            RefreshProperties();
            user = (Application.Current as App)!.AppUser;
            passwordRepeat = string.Empty;
            RegisterCommand = new Command(async () => await Register());
            ToggleIsPassword1Command = new Command(ToggleIsPassword1);
            ToggleIsPassword2Command = new Command(ToggleIsPassword2);
        }
        private void ToggleIsPassword1()
        {
            IsPassword1 = !IsPassword1;
            OnPropertyChanged(nameof(IsPassword1));
        }
        private void ToggleIsPassword2()
        {
            IsPassword2 = !IsPassword2;
            OnPropertyChanged(nameof(IsPassword2));
        }
        private bool CanRegister()
        {
            return user.CanRegister(passwordRepeat);
        }
        private async Task Register()
        {
            if (CanRegister())
            {
                IsBusy = true;
                OnPropertyChanged(nameof(IsBusy));
                RegisterEnable = false;
                OnPropertyChanged(nameof(RegisterEnable));
                bool successfullyRegistered = await user.Register();
                if (successfullyRegistered)
                    _ = Shell.Current.Navigation.PushAsync(new MainPage());
                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
                RegisterEnable = false;
                OnPropertyChanged(nameof(RegisterEnable));
            }
        }
        private async void GetRandomUsername(object obj)// cannot be sync because of http method
        {
            // Create an instance of RandomUsername to call the instance method GetAsync
            RandomUsername randomUsername = new();
            UserName = await randomUsername.GetAsync();
            OnPropertyChanged(nameof(UserName));
        }
        private void RefreshProperties()
        {
            IsPassword1 = true;
            IsPassword2 = true;
            SeveralPropertiesChange();
        }
        private void SeveralPropertiesChange()
        {
            string[] nameOfs = [nameof(UserName), nameof(Email), nameof(Password), nameof(IsPassword1), nameof(IsPassword2)];
            for (int i = 0; i < nameOfs.Length; i++)
                OnPropertyChanged(nameOfs[i]);
        }
        private void NavToLogin()
        {
            Shell.Current.Navigation.PushAsync(new LoginPage());
        }
        private void NavHome()
        {
            Shell.Current.Navigation.PushAsync(new MainPage());
        }
    }
}
