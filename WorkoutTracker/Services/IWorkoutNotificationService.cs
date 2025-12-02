namespace WorkoutTracker.Services
{
    public interface IWorkoutNotificationService
    {
        Task PlayCountdownTickAsync();
        Task PlayPhaseChangeAsync(bool isWorkPhase);
    }
}
