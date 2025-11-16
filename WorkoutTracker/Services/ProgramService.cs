using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Models.Program>> GetAllProgramsAsync()
        {
            try
            {
                return await _context.Programs
                    .Include(p => p.Exercises)
                    .ThenInclude(pe => pe.Exercise)
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке программ: {ex.Message}");
                return new List<Program>();
            }
        }

        public async Task<Models.Program> GetProgramByIdAsync(int id)
        {
            try
            {
                return await _context.Programs
                    .Include(p => p.Exercises)
                    .ThenInclude(pe => pe.Exercise)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке программы: {ex.Message}");
                return null;
            }
        }

        public async Task<int> SaveProgramAsync(Program program)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                if (program.Id == 0)
                {
                    
                    var programExercises = new List<ProgramExercise>();

                    foreach (var pe in program.Exercises)
                    {
                        
                        var exercise = await _context.Exercises.FindAsync(pe.ExerciseId);

                        if (exercise == null)
                        {
                            throw new InvalidOperationException($"Упражнение с Id={pe.ExerciseId} не найдено в базе данных");
                        }

                        
                        programExercises.Add(new ProgramExercise
                        {
                            ExerciseId = pe.ExerciseId,
                            Exercise = exercise, 
                            Approaches = pe.Approaches
                        });
                    }

                    var newProgram = new Program
                    {
                        Name = program.Name,
                        Exercises = programExercises
                    };

                    _context.Programs.Add(newProgram);
                    await _context.SaveChangesAsync();

                    program.Id = newProgram.Id;
                }
                else
                {
                    var existingProgram = await _context.Programs
                        .Include(p => p.Exercises)
                        .FirstOrDefaultAsync(p => p.Id == program.Id);

                    if (existingProgram != null)
                    {
                        existingProgram.Name = program.Name;

                        _context.ProgramExercises.RemoveRange(existingProgram.Exercises);

                        foreach (var pe in program.Exercises)
                        {
                            var exercise = await _context.Exercises.FindAsync(pe.ExerciseId);

                            if (exercise == null)
                            {
                                throw new InvalidOperationException($"Упражнение с Id={pe.ExerciseId} не найдено в базе данных");
                            }

                            existingProgram.Exercises.Add(new ProgramExercise
                            {
                                ExerciseId = pe.ExerciseId,
                                Exercise = exercise,
                                Approaches = pe.Approaches
                            });
                        }

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new InvalidOperationException($"Программа с Id={program.Id} не найдена");
                    }
                }

                await transaction.CommitAsync();
                return program.Id;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при сохранении программы: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return 0;
            }
        }

        public async Task<bool> DeleteProgramAsync(int id)
        {
            try
            {
                var program = await _context.Programs
                    .Include(p => p.Exercises)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (program == null)
                    return false;

                _context.Programs.Remove(program);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при удалении программы: {ex.Message}");
                return false;
            }
        }
    }
}
