using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class JoinableGamesListModel
    {
        protected readonly FbData fbd = new();
        public ObservableCollection<JoinableGame>? gamesObsCollection;
        protected IListenerRegistration? ilr;
        public EventHandler? OnGamesChanged;
        public abstract void AddSnapshotListener();
        public abstract void RemoveSnapshotListener();
    }
}
