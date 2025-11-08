namespace WorkoutTracker.Services
{
    public class NavigationService : INavigationService
    {
        public async Task NavigateBackAsync()
        {
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка навигации: {ex.Message}");
            }
        }

        public async Task ShowWorkoutCompletedAlertAsync()
        {
            try
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Тренировка завершена!",
                        "Поздравляем! Вы завершили тренировку.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при показе алерта: {ex.Message}");
            }
        }
    }
}
