using Microsoft.EntityFrameworkCore;
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
            try
            {
                return await _context.Exercises
                    .OrderBy(e => e.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке упражнений: {ex.Message}");
                return new List<Exercise>();
            }
        }

        public async Task<List<Exercise>> GetCustomExercisesAsync()
        {
            try
            {
                return await _context.Exercises
                    .Where(e => e.IsCustom)
                    .OrderBy(e => e.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке пользовательских упражнений: {ex.Message}");
                return new List<Exercise>();
            }
        }

        public async Task<List<Exercise>> GetSystemExercisesAsync()
        {
            try
            {
                return await _context.Exercises
                    .Where(e => !e.IsCustom)
                    .OrderBy(e => e.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке системных упражнений: {ex.Message}");
                return new List<Exercise>();
            }
        }

        public async Task<Exercise> GetExerciseByIdAsync(int id)
        {
            try
            {
                return await _context.Exercises
                    .FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке упражнения: {ex.Message}");
                return null;
            }
        }

        public async Task<int> SaveExerciseAsync(Exercise exercise)
        {
            try
            {
                if (exercise.Id == 0)
                {
                    _context.Exercises.Add(exercise);
                }
                else
                {
                    _context.Exercises.Update(exercise);
                }

                await _context.SaveChangesAsync();
                return exercise.Id;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при сохранении упражнения: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> DeleteExerciseAsync(int id)
        {
            try
            {
                var exercise = await _context.Exercises.FindAsync(id);
                if (exercise == null)
                    return false;

                var isUsed = await _context.ProgramExercises
                    .AnyAsync(pe => pe.ExerciseId == id);

                if (isUsed)
                {
                    System.Diagnostics.Debug.WriteLine($"Невозможно удалить упражнение, так как оно используется в программах.");
                    return false;
                }

                _context.Exercises.Remove(exercise);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при удалении упражнения: {ex.Message}");
                return false;
            }
        }
    }
}