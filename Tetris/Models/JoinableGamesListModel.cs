using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using Tetris.Interfaces;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    /// <summary>
    /// Represents an abstract model for managing and observing a list of joinable games, providing methods for
    /// asynchronous creation, database interaction, and event-driven updates.
    /// </summary>
    public abstract class JoinableGamesListModel
    {
        #region Fields
        protected IListenerRegistration? ilr;
        protected readonly FbData fbd = IPlatformApplication.
            Current?.Services.GetService<IFbData>() as FbData ?? new();
        #endregion

        #region Properties
        public ObservableCollection<Game>? GamesObsCollection { get; set; }
        #endregion

        #region Events
        public EventHandler? OnGamesChanged;
        #endregion
    
        #region Public Methods
        public abstract Task<JoinableGamesList> CreateAsync();
        public abstract void AddGamesCollectionListener();
        public abstract void RemoveGamesCollectionListener();
        public abstract void AddGameToDB(Game currentNewGame, User creator);
        #endregion
        
        #region Protected Methods
        protected abstract void OnChange(IQuerySnapshot snapshot, Exception error);
        protected abstract void OnCompleteChange(ObservableCollection<Game> newList);
        #endregion
    }
}
