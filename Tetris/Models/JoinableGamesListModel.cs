using Plugin.CloudFirestore;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    abstract class JoinableGamesListModel
    {
        protected readonly FbData fbd = new();
        public List<JoinableGame>? list;
        protected IListenerRegistration? ilr;
        public abstract void AddSnapshotListener();
        public abstract void RemoveSnapshotListener();
    }
}
