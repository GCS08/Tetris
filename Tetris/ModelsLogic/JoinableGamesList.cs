using Plugin.CloudFirestore;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    internal class JoinableGamesList : JoinableGamesListModel
    {
        public JoinableGamesList(List<JoinableGame> list)
        {
            this.list = list;
        }
        public static async Task<JoinableGamesList> CreateAsync()
        {
            // create an instance so we can access fbd from the base class
            JoinableGamesList innerObject = new([]);
            innerObject.list = await innerObject.fbd.GetJoinableGamesAsync();
            return innerObject;
        }
        public override void AddSnapshotListener()
        {
            ilr = fbd.AddSnapshotListener(Keys.GamesCollectionName, OnChange!);
        }
        private void OnChange(IQuerySnapshot snapshot, Exception error)
        {
            fbd.GetDocumentsWhereDiffValue(Keys.GamesCollectionName,
                Keys.CurrentPlayersCountKey, Keys.MaxPlayersCountKey, OnComplete);
        }
        private void OnComplete(List<JoinableGame> newList)
        {
            list!.Clear();
            foreach (JoinableGame game in newList) { list.Add(game); }
            OnGamesChanged?.Invoke(this, EventArgs.Empty);
        }
        public override void RemoveSnapshotListener()
        {
            ilr?.Remove();
        }
    }
}
