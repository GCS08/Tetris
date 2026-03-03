using Tetris.ModelsLogic;
namespace Tetris.Models
{
    public abstract class RandomUsernameModel
    {
        protected string apiUrl = Keys.RandomUsernameApiUrl;
        protected HttpClient client = new();
        protected HttpResponseMessage? response;
        protected string? json;
        public abstract Task<string> GetAsync();
    }
}
