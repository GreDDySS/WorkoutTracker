using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly WorkoutDbContext _context;

        public ExerciseService(WorkoutDbContext context)
        {
            _context = context;
        }

        public async Task<List<Exercise>> GetAllExercisesAsync()
        {
            // TODO: Реализовать загрузку из БД
            return new List<Exercise>();
        }

        public async Task<List<Exercise>> GetCustomExercisesAsync()
        {
            // TODO: Реализовать загрузку из БД
            return new List<Exercise>();
        }

        public async Task<List<Exercise>> GetSystemExercisesAsync()
        {
            // TODO: Реализовать загрузку из БД
            // Временные данные для примера
            return new List<Exercise>
            {
                new Exercise { Id = 101, Name = "Отжимания", WorkTimeSeconds = 20, RestTimeSeconds = 10, IsCustom = false },
                new Exercise { Id = 102, Name = "Приседания", WorkTimeSeconds = 25, RestTimeSeconds = 15, IsCustom = false }
            };
        }

        public async Task<Exercise> GetExerciseByIdAsync(int id)
        {
            // TODO: Реализовать загрузку из БД
            return null;
        }

        public async Task<int> SaveExerciseAsync(Exercise exercise)
        {
            // TODO: Реализовать сохранение в БД
            System.Diagnostics.Debug.WriteLine($"Сохранение упражнения: {exercise.Name}, Работа: {exercise.WorkTimeSeconds}с, Отдых: {exercise.RestTimeSeconds}с");
            return 0;
        }

        public async Task<bool> DeleteExerciseAsync(int id)
        {
            // TODO: Реализовать удаление из БД
            return false;
        }
    }
}