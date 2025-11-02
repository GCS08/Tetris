using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tetris.Models;

namespace Tetris.ViewModels
{
    internal class NewGameConfigPageVM : ObservableObject
    {
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand PickerFocusedCommand => new Command(PickerFocused);
        public ICommand PickerUnfocusedCommand => new Command(PickerUnfocused);
        private Color pickerTitleColor;
        public Color PickerTitleColor
        {
            get => pickerTitleColor;
            set
            {
                if (pickerTitleColor != value)
                {
                    pickerTitleColor = value;
                    OnPropertyChanged(nameof(PickerTitleColor));
                }
            }
        }
        public NewGameConfigPageVM()
        {
            pickerTitleColor = Colors.White;
        }
        private void PickerFocused()
        {
            PickerTitleColor = Colors.Black;
        }
        private void PickerUnfocused()
        {
            PickerTitleColor = Colors.White;
        }
        private async void NavHome()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectMainPageRefresh);
        }
    }
}
