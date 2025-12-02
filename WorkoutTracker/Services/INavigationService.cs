namespace WorkoutTracker.Services
{
    public interface INavigationService
    {
        Task NavigateBackAsync();
        Task NavigateTo(string page);
        Task ShowWorkoutCompletedAlertAsync();
    }

}
