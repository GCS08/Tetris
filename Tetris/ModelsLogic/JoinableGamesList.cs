using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class JoinableGamesList : JoinableGamesListModel
    {
        #region Constructors

        /// <summary>
        /// Default constructor for JoinableGamesList.
        /// </summary>
        public JoinableGamesList() { }

        /// <summary>
        /// Initializes a new instance of <see cref="JoinableGamesList"/> with an existing collection of games.
        /// </summary>
        /// <param name="list">
        /// The observable collection of <see cref="Game"/> objects to manage.
        /// </param>
        public JoinableGamesList(ObservableCollection<Game> list)
        {
            this.GamesObsCollection = list;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Asynchronously creates a new <see cref="JoinableGamesList"/> instance
        /// and populates it with the list of currently available games from Firestore.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{JoinableGamesList}"/> representing the asynchronous operation.
        /// The result contains the initialized <see cref="JoinableGamesList"/> instance.
        /// </returns>
        public override async Task<JoinableGamesList> CreateAsync()
        {
            // create an instance so we can access fbd from the base class
            JoinableGamesList innerObject = new([]);
            innerObject.GamesObsCollection = await innerObject.fbd.GetAvailGamesList();
            return innerObject;
        }

        /// <summary>
        /// Adds a Firestore listener to the games collection to react to any changes.
        /// </summary>
        public override void AddGamesCollectionListener()
        {
            ilr = fbd.AddGamesCollectionListener(OnChange);
        }

        /// <summary>
        /// Removes the Firestore listener for the games collection.
        /// </summary>
        public override void RemoveGamesCollectionListener()
        {
            ilr?.Remove();
        }

        /// <summary>
        /// Adds a new game to the Firestore database.
        /// </summary>
        /// <param name="currentNewGame">
        /// The <see cref="Game"/> object to add to the database.
        /// </param>
        /// <param name="creator">
        /// The <see cref="User"/> object representing the creator of the game.
        /// </param>
        public override void AddGameToDB(Game currentNewGame, User creator)
        {
            if (currentNewGame.GameBoard == null || currentNewGame.GameBoard.CurrentShape == null
                || currentNewGame.GameBoard.CurrentShape.Color == null) return;

            currentNewGame.GameID = fbd.AddGameToDB(
                creator.UserID,
                creator.UserName,
                currentNewGame.CubeColor,
                currentNewGame.CurrentPlayersCount,
                currentNewGame.MaxPlayersCount,
                currentNewGame.IsFull,
                currentNewGame.GameBoard.CurrentShape.Id,
                Converters.StringAndColorConverter.ColorToColorName
                    (currentNewGame.GameBoard.CurrentShape.Color),
                currentNewGame.FirstShapesList,
                currentNewGame.IsPublicGame);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Callback for Firestore snapshot changes in the games collection.
        /// Fetches the updated list of available games and updates the observable collection.
        /// </summary>
        /// <param name="snapshot">
        /// The Firestore query snapshot containing current game documents.
        /// May be null if an error occurred.
        /// </param>
        /// <param name="error">
        /// Any exception that occurred while retrieving the snapshot.
        /// </param>
        protected override void OnChange(IQuerySnapshot? snapshot, Exception? error)
        {
            fbd.GetAvailGames(OnCompleteChange);
        }

        /// <summary>
        /// Called when the list of available games has been fetched from Firestore.
        /// Updates the observable collection and raises the OnGamesChanged event.
        /// </summary>
        /// <param name="newList">
        /// The collection of <see cref="Game"/> objects retrieved from Firestore.
        /// </param>
        protected override void OnCompleteChange(ObservableCollection<Game> newList)
        {
            if (GamesObsCollection == null) return;

            GamesObsCollection.Clear();
            foreach (Game game in newList)
                GamesObsCollection.Add(game);

            OnGamesChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}