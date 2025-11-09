using WorkoutTracker.Models;

namespace WorkoutTracker.Services
{
    public interface IProgramService
    {
        Task<List<Program>> GetAllProgramsAsync();
        Task<Program> GetProgramByIdAsync(int id);
        Task<int> SaveProgramAsync(Program program);
        Task<bool> DeleteProgramAsync(int id);
    }
}
