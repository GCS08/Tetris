using Plugin.CloudFirestore;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    internal class JoinableGamesList : JoinableGamesListModel
    {
        public JoinableGamesList()
        {
            list = fbd.GetJoinableGames();
        }

        public override void AddSnapshotListener()
        {
            ilr = fbd.AddSnapshotListener(Keys.GamesCollectionName, OnChange);
        }
        private override void OnChange(IQuerySnapshot snapshot, Exception error)
        {
            fbd.GetDocumentsWhereDiffValue(Keys.GamesCollectionName,
                Keys.CurrentPlayersCountKey, Keys.MaxPlayersCountKey, OnComplete);
        }
        private void OnComplete(IQuerySnapshot qs)
        {
            list!.Clear();
            foreach (IDocumentSnapshot ds in qs.Documents)
            {
                JoinableGame? game = ds.ToObject<JoinableGame>();
                if (game != null)
                {
                    game.GameID = ds.Id;
                    list.Add(game);
                }
            }
            OnGamesChanged?.Invoke(this, EventArgs.Empty);
        }
        public override void RemoveSnapshotListener()
        {
            throw new NotImplementedException();
        }
    }
}
