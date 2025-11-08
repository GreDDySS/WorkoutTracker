namespace WorkoutTracker.Services
{
    public class WorkoutTimerService : IWorkoutTimerService, IDisposable
    {
        private System.Timers.Timer _timer;
        private bool _isPaused;

        public event EventHandler<int> TimeElapsed;
        public event EventHandler PhaseCompleted;

        public void Start()
        {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimeElapsed;
            _timer.AutoReset = true;
            _timer.Start();

        }

        public void Pause() => _isPaused = true;
        public void Resume() => _isPaused = false;
        public void Stop() => _timer?.Stop();

        public void OnTimeElapsed(object sender, EventArgs e)
        {
            if (!_isPaused)
            {
                TimeElapsed?.Invoke(this, 1);
            }
        }

        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();

        }
    }
}
