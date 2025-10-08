using Tetris.ModelsLogic;

namespace Tetris
{
    public partial class App : Application
    {
        public User user = new();
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
