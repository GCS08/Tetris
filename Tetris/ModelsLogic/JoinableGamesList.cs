using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class JoinableGamesList : JoinableGamesListModel
    {
        public JoinableGamesList(ObservableCollection<Game> list)
        {
            this.gamesObsCollection = list;
        }
        public static async Task<JoinableGamesList> CreateAsync()
        {
            // create an instance so we can access fbd from the base class
            JoinableGamesList innerObject = new([]);
            innerObject.gamesObsCollection = await innerObject.fbd.GetDocumentsWhereDiffValue(Keys.GamesCollectionName,
                Keys.CurrentPlayersCountKey, Keys.MaxPlayersCountKey);
            return innerObject;
        }
        public override void AddSnapshotListener()
        {
            ilr = fbd.AddSnapshotListener(Keys.GamesCollectionName, OnChange!);
        }
        private void OnChange(IQuerySnapshot snapshot, Exception error)
        {
            fbd.GetDocumentsWhereDiffValue(Keys.GamesCollectionName,
                Keys.CurrentPlayersCountKey, Keys.MaxPlayersCountKey, OnCompleteChange);
        }
        private void OnCompleteChange(ObservableCollection<Game> newList)
        {
            gamesObsCollection!.Clear();
            foreach (Game game in newList) { gamesObsCollection.Add(game); }
            OnGamesChanged?.Invoke(this, EventArgs.Empty);
        }
        public override void RemoveSnapshotListener()
        {
            ilr?.Remove();
        }

        public async Task AddGameToDB(Game currentNewGame)
        {
            currentNewGame.GameID = await fbd.AddGameToDB(
                currentNewGame.CubeColor,
                Preferences.Get(Keys.UserNameKey, string.Empty),
                currentNewGame.CurrentPlayersCount,
                currentNewGame.MaxPlayersCount,
                currentNewGame.IsPublicGame);
        }
    }
}
