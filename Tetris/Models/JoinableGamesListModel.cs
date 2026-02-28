using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using Tetris.Interfaces;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class JoinableGamesListModel
    {
        public ObservableCollection<Game>? gamesObsCollection;
        public EventHandler? OnGamesChanged;
        protected IListenerRegistration? ilr;
        protected readonly FbData fbd = IPlatformApplication.Current?.Services.GetService<IFbData>() as FbData ?? new();
        public abstract Task<JoinableGamesList> CreateAsync();
        public abstract void AddGamesCollectionListener();
        protected abstract void OnChange(IQuerySnapshot snapshot, Exception error);
        protected abstract void OnCompleteChange(ObservableCollection<Game> newList);
        public abstract void RemoveGamesCollectionListener();
        public abstract void AddGameToDB(Game currentNewGame, User creator);
    }
}
