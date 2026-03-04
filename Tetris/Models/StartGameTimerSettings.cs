namespace Tetris.Models
{
    /// <summary>
    /// Represents timer settings for starting a game, including total duration and interval values in milliseconds.
    /// </summary>
    /// <param name="totalTimeInMilliseconds">The total duration of the timer in milliseconds.</param>
    /// <param name="intervalInMilliseconds">The interval at which the timer ticks in milliseconds.</param>
    public class StartGameTimerSettings(long totalTimeInMilliseconds, long intervalInMilliseconds)
    {
        #region Properties
        public long TotalTimeInMilliseconds { get; set; } = totalTimeInMilliseconds;
        public long IntervalInMilliseconds { get; set; } = intervalInMilliseconds;
        #endregion
    }
}
