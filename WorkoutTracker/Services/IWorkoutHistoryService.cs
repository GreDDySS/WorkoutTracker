using WorkoutTracker.Models;

namespace WorkoutTracker.Services
{
    public interface IWorkoutHistoryService
    {
        Task<int> SaveWorkoutAsync(WorkoutHistory workout);
        Task<List<WorkoutHistory>> GetWorkoutsByDateAsync(DateTime date);
        Task<List<WorkoutHistory>> GetWorkoutsByMonthAsync(int year, int month);
        Task<Dictionary<DateTime, int>> GetWorkoutDaysInMonthAsync(int year, int month);
    }
}

