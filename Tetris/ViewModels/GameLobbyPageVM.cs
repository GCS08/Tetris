using System.Collections.ObjectModel;
using System.Windows.Input;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    public partial class GameLobbyPageVM : ObservableObject
    {
        private readonly Notifications notifications;
        private JoinableGamesList? JoinableGamesList { get; set; } = new();
        public ObservableCollection<Game>? Games { get; private set; }
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand NavToGameCommand => new Command(NavHome);
        public ICommand NavToNewGameConfigCommand => new Command(NavToNewGameConfigGame);
        public ICommand SendNotificationCommand { get; private set; }
        public GameLobbyPageVM()
        {
            notifications = new Notifications();
            SendNotificationCommand = new Command(SendNotification);
        }
        private void SendNotification(object obj)
        {
            notifications.PushNotification("Notification", "My notification message... ");
        }
        private void OnGamesChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Games));
        }
        public void AddGamesCollectionListener()
        {
            if (JoinableGamesList == null) return;
            JoinableGamesList.AddGamesCollectionListener();
        }
        public void RemoveGamesCollectionListener()
        {
            if (JoinableGamesList == null) return;
            JoinableGamesList.RemoveGamesCollectionListener();
        }
        private void NavHome()
        {
            Shell.Current.Navigation.PushAsync(new MainPage());
        }
        private void NavToNewGameConfigGame()
        {
            if (JoinableGamesList == null) return;
            Shell.Current.Navigation.PushAsync(new NewGameConfigPage(JoinableGamesList));
        }
        public async Task LoadGamesList() 
        {
            if (JoinableGamesList == null) return;
            JoinableGamesList = await JoinableGamesList.CreateAsync();
            JoinableGamesList.OnGamesChanged += OnGamesChanged;
            Games = JoinableGamesList.gamesObsCollection;
            OnPropertyChanged(nameof(Games));
        }
    }
}
