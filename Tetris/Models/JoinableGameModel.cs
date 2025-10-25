namespace Tetris.Models
{
    public class JoinableGameModel
    {
        public string CubeColor { get; set; } = "Transparent";
        public string CreatorName { get; set; } = "Anonymous";
        public int CurrentPlayersCount { get; set; } = 0;
        public int MaxPlayersCount { get; set; } = 4;
        public string GameID { get; set; } = string.Empty;
    }
}
