namespace Tetris.Models
{
    /// <summary>
    /// Represents an abstract base class for retrieving random usernames from an external API.
    /// </summary>
    public abstract class RandomUsernameModel
    {
        #region Fields
        protected string apiUrl = Keys.RandomUsernameApiUrl;
        protected HttpClient client = new();
        protected HttpResponseMessage? response;
        protected string? json;
        #endregion

        #region Public Methods
        public abstract Task<string> GetAsync();
        #endregion
    }
}
