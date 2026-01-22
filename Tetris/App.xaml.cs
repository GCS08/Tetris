using Tetris.ModelsLogic;

namespace Tetris
{
    public partial class App : Application
    {
        public User AppUser { get; set; } = new();
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}
