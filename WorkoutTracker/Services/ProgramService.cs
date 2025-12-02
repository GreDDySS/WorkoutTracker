using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Services
{
    public class ProgramService : IProgramService
    {
        private readonly WorkoutDbContext _context;

        public ProgramService(WorkoutDbContext context) => _context = context;

        public async Task<List<Program>> GetAllProgramsAsync() =>
            await _context.Programs
                .Include(p => p.Exercises)
                .ThenInclude(pe => pe.Exercise)
                .OrderBy(p => p.Name)
                .ToListAsync();

        public async Task<Models.Program?> GetProgramByIdAsync(int id) =>
            await _context.Programs
                .AsNoTracking()
                .Include(p => p.Exercises)
                .ThenInclude(pe => pe.Exercise)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<int> SaveProgramAsync(Program program)
        {
            if (string.IsNullOrWhiteSpace(program.Name))
                return 0;

            var exerciseIds = program.Exercises.Select(pe => pe.ExerciseId).Distinct().ToList();
            if (exerciseIds.Any())
            {
                var existingCount = await _context.Exercises
                    .CountAsync(e => exerciseIds.Contains(e.Id));

                if (existingCount != exerciseIds.Count)
                    return 0;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (program.Id == 0)
                {
                    var newProgram = new Program { Name = program.Name };

                    newProgram.Exercises.AddRange(program.Exercises.Select(pe => new ProgramExercise
                    {
                        ExerciseId = pe.ExerciseId,
                        Approaches = pe.Approaches
                    }));

                    _context.Programs.Add(newProgram);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return newProgram.Id;
                }
                else
                {
                    var existingProgram = await _context.Programs
                        .Include(p => p.Exercises)
                        .FirstOrDefaultAsync(p => p.Id == program.Id);

                    if (existingProgram == null)
                        return 0;

                    existingProgram.Name = program.Name;
                    existingProgram.Exercises.Clear();

                    existingProgram.Exercises.AddRange(program.Exercises.Select(pe => new ProgramExercise
                    {
                        ProgramId = existingProgram.Id,
                        ExerciseId = pe.ExerciseId,
                        Approaches = pe.Approaches
                    }));

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return existingProgram.Id;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ProgramService] SaveProgramAsync ФАТАЛЬНАЯ ОШИБКА: {ex.InnerException?.Message ?? ex.Message}\n{ex.StackTrace}");
                await transaction.RollbackAsync();
                return 0;
            }
        }

        public async Task<bool> DeleteProgramAsync(int id)
        {
            var program = await _context.Programs.FindAsync(id);
            if (program == null) return false;

            _context.Programs.Remove(program);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
