using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Services
{
    public class ProgramService : IProgramService
    {
        private readonly WorkoutDbContext _context;

        public ProgramService(WorkoutDbContext context)
        {
            _context = context;
        }

        public async Task<List<Program>> GetAllProgramsAsync()
        {
            // TODO: Реализовать загрузку из БД
            return new List<Program>();
        }

        public async Task<Program> GetProgramByIdAsync(int id)
        {
            // TODO: Реализовать загрузку из БД
            return null;
        }

        public async Task<int> SaveProgramAsync(Program program)
        {
            // TODO: Реализовать сохранение в БД
            System.Diagnostics.Debug.WriteLine($"Сохранение программы: {program.Name}");
            System.Diagnostics.Debug.WriteLine($"Упражнений: {program.Exercises.Count}");
            foreach (var ex in program.Exercises)
            {
                System.Diagnostics.Debug.WriteLine($"  - {ex.Exercise.Name}: {ex.Approaches} подход(ов)");
            }
            return 0;
        }

        public async Task<bool> DeleteProgramAsync(int id)
        {
            // TODO: Реализовать удаление из БД
            return false;
        }
    }
}
