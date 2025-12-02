using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly WorkoutDbContext _context;

        public ExerciseService(WorkoutDbContext context) => _context = context;

        public async Task<List<Exercise>> GetAllExercisesAsync()
            => await _context.Exercises.OrderBy(e => e.Name).ToListAsync();

        public async Task<List<Exercise>> GetCustomExercisesAsync()
            => await _context.Exercises.Where(e => e.IsCustom).OrderBy(e => e.Name).ToListAsync();

        public async Task<List<Exercise>> GetSystemExercisesAsync()
            => await _context.Exercises.Where(e => !e.IsCustom).OrderBy(e => e.Name).ToListAsync();

        public async Task<Exercise?> GetExerciseByIdAsync(int id)
            => await _context.Exercises.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

        public async Task<int> SaveExerciseAsync(Exercise exercise)
        {
            try
            {
                if (exercise.Id == 0)
                {
                    exercise.IsCustom = true;
                    _context.Exercises.Add(exercise);
                }
                else
                {
                    var existing = await _context.Exercises.FindAsync(exercise.Id);
                    if (existing == null)
                        return 0;

                    _context.Entry(existing).CurrentValues.SetValues(exercise);
                    exercise.IsCustom = exercise.IsCustom;
                }

                await _context.SaveChangesAsync();
                return exercise.Id;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExerciseService] SaveExerciseAsync ошибка: {ex.Message}\n{ex.StackTrace}");
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

                if (!exercise.IsCustom) return false;

                var isUsed = await _context.ProgramExercises.AnyAsync(pe => pe.ExerciseId == id);

                if (isUsed)
                {
                    System.Diagnostics.Debug.WriteLine($"Упражнение Id={id} используется в программах — удаление запрещено");
                    return false;
                }

                _context.Exercises.Remove(exercise);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExerciseService] DeleteExerciseAsync ошибка: {ex.Message}");
                return false;
            }
        }
    }
}