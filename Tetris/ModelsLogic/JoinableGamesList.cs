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
        public JoinableGamesList()
        {
        }
        public override async Task<JoinableGamesList> CreateAsync()
        {
            // create an instance so we can access fbd from the base class
            JoinableGamesList innerObject = new([]);
            innerObject.gamesObsCollection = await innerObject.fbd.GetAvailGamesList();
            return innerObject;
        }
        public override void AddGamesCollectionListener()
        {
            ilr = fbd.AddGamesCollectionListener(OnChange!);
        }
        protected override void OnChange(IQuerySnapshot snapshot, Exception error)
        {
            fbd.GetAvailGames(OnCompleteChange);
        }
        protected override void OnCompleteChange(ObservableCollection<Game> newList)
        {
            gamesObsCollection!.Clear();
            foreach (Game game in newList) { gamesObsCollection.Add(game); }
            OnGamesChanged?.Invoke(this, EventArgs.Empty);
        }
        public override void RemoveGamesCollectionListener()
        {
            ilr?.Remove();
        }
        public override async Task AddGameToDB(Game currentNewGame, User creator)
        {
            currentNewGame.GameID = await fbd.AddGameToDB(
                creator.UserID,
                creator.UserName,
                currentNewGame.CubeColor,
                currentNewGame.CurrentPlayersCount,
                currentNewGame.MaxPlayersCount,
                currentNewGame.IsFull,
                currentNewGame.GameBoard!.CurrentShape!.Id!,
                currentNewGame.GameBoard!.CurrentShape!.InGameId!,
                Converters.StringAndColorConverter.ColorToColorName
                (currentNewGame.GameBoard!.CurrentShape!.Color!),
                currentNewGame.IsPublicGame);
        }
    }
}
