namespace WorkoutTracker.Services
{
    public interface INavigationService
    {
        Task NavigateBackAsync();
        Task ShowWorkoutCompletedAlertAsync();
    }

}
