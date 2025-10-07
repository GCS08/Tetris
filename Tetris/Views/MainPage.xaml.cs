using Tetris.ViewModels;

namespace Tetris.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageVM();
    }
}

