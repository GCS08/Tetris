using Firebase.Auth;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.ViewModels
{
    public class GameLobbyPageVM : ObservableObject
    {
        private JoinableGamesList? JoinableGamesList { get; set; }
        public ObservableCollection<JoinableGame>? Games { get; private set; }
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand NavToGameCommand => new Command(NavToGame);
        public ICommand NavToNewGameConfigCommand => new Command(NavToNewGameConfigGame);
        private void OnGamesChanged(object? sender, EventArgs e)
        {
            Games!.Clear();
            foreach (var game in JoinableGamesList!.list!)
            {
                Games.Add(game);
            }
        }
        public void AddSnapshotListener()
        {
            JoinableGamesList!.AddSnapshotListener();
        }
        public void RemoveSnapshotListener()
        {
            JoinableGamesList!.RemoveSnapshotListener();
        }
        private async void NavHome()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectMainPageRefresh);
        }
        private async void NavToGame()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectGamePage);
        }
        private async void NavToNewGameConfigGame()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectNewGameConfigPage);
        }

        public async Task LoadGamesList()
        {
            // Initialize the joinable games list
            JoinableGamesList = await JoinableGamesList.CreateAsync();

            // Subscribe to changes
            JoinableGamesList!.OnGamesChanged += OnGamesChanged;

            // Populate the ObservableCollection
            Games!.Clear(); // clear any existing items
            foreach (var game in JoinableGamesList.list!)
            {
                Games.Add(game);
            }
        }

    }
}
