using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    internal class RegisterPageVM : ObservableObject, IQueryAttributable
    {
        public ICommand NavToLoginCommand => new Command(NavToLogin);
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand RegisterCommand { get; }
        public ICommand ToggleIsPasswordCommand { get; }
        public bool IsBusy { get; set; } = false;
        private App? app;
        private User user;
        public string UserName
        {
            get => user.UserName;
            set
            {
                user.UserName = value;
                (RegisterCommand as Command)?.ChangeCanExecute();
            }
        }
        public string Email
        {
            get => user.Email;
            set
            {
                user.Email = value;
                (RegisterCommand as Command)?.ChangeCanExecute();
            }
        }
        public string Password
        {
            get => user.Password;
            set
            {
                user.Password = value;
                (RegisterCommand as Command)?.ChangeCanExecute();
            }
        }
        public bool IsPassword { get; set; } = true;
        public RegisterPageVM()
        {
            app = Application.Current as App;
            user = app!.user;
            RegisterCommand = new Command(Register, CanRegister);
            ToggleIsPasswordCommand = new Command(ToggleIsPassword);
        }
        private void ToggleIsPassword()
        {
            IsPassword = !IsPassword;
            OnPropertyChanged(nameof(IsPassword));
        }
        private bool CanRegister()
        {
            return user.CanRegister();
        }
        private async void Register()
        {
            IsBusy = true;
            OnPropertyChanged(nameof(IsBusy));
            bool successfullyRegistered = await user.Register();
            IsBusy = false;
            OnPropertyChanged(nameof(IsBusy));
            if (successfullyRegistered)
                await Shell.Current.GoToAsync("///MainPage?refresh=true");
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
            string[] nameOfs = { nameof(UserName), nameof(Email), nameof(Password), nameof(IsPassword) };
            for (int i = 0; i < nameOfs.Length; i++)
                OnPropertyChanged(nameOfs[i]);
        }
        private async void NavToLogin()
        {
            await Shell.Current.GoToAsync("///LoginPage");
        }
        private async void NavHome()
        {
            await Shell.Current.GoToAsync("///MainPage?refresh=true");
        }
    }
}
