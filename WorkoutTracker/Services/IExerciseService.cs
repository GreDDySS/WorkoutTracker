using WorkoutTracker.Models;

namespace WorkoutTracker.Services
{
    public interface IExerciseService
    {
        Task<List<Exercise>> GetAllExercisesAsync();
        Task<List<Exercise>> GetCustomExercisesAsync();
        Task<List<Exercise>> GetSystemExercisesAsync();
        Task<Exercise?> GetExerciseByIdAsync(int id);
        Task<int> SaveExerciseAsync(Exercise exercise);
        Task<bool> DeleteExerciseAsync(int id);
    }
}
