using System.Collections.ObjectModel;
using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    public partial class GameLobbyPageVM : ObservableObject
    {
        private JoinableGamesList? JoinableGamesList { get; set; } = new();
        public ObservableCollection<Game>? Games { get; private set; }
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand NavToGameCommand => new Command(NavHome);
        public ICommand NavToNewGameConfigCommand => new Command(NavToNewGameConfigGame);
        private void OnGamesChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Games));
        }
        public void AddGamesCollectionListener()
        {
            JoinableGamesList!.AddGamesCollectionListener();
        }
        public void RemoveGamesCollectionListener()
        {
            JoinableGamesList!.RemoveGamesCollectionListener();
        }
        private async void NavHome()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectMainPageRefresh);
        }
        private async void NavToNewGameConfigGame()
        {
            await Shell.Current.Navigation.PushAsync(new NewGameConfigPage(JoinableGamesList!));
        }
        public async Task LoadGamesList()
        {
            JoinableGamesList = await JoinableGamesList!.CreateAsync();
            JoinableGamesList!.OnGamesChanged += OnGamesChanged;
            Games = JoinableGamesList.gamesObsCollection;
            OnPropertyChanged(nameof(Games));
        }
    }
}
