namespace Tetris.Models
{
    public class StartGameTimerSettings(long totalTimeInMilliseconds, long intervalInMilliseconds)
    {
        public long TotalTimeInMilliseconds { get; set; } = totalTimeInMilliseconds;
        public long IntervalInMilliseconds { get; set; } = intervalInMilliseconds;
    }
}
