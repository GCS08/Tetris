using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    internal class RegisterPageVM : ObservableObject
    {
        public ICommand RegisterCommand { get; }
        public ICommand NavToLoginCommand => new Command(NavToLogin);
        public ICommand ToggleIsPasswordCommand { get; }
        public bool IsBusy { get; set; } = false;
        private User user = new();
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
            await user.Register();
            IsBusy = false;
            OnPropertyChanged(nameof(IsBusy));
        }
        private async void NavToLogin()
        {
            await Shell.Current.GoToAsync("///LoginPage");
        }
    }
}
