using WorkoutTracker.Models;

namespace WorkoutTracker.Services
{
    public interface IProgramService
    {
        Task<List<Models.Program>> GetAllProgramsAsync();
        Task<Models.Program?> GetProgramByIdAsync(int id);
        Task<int> SaveProgramAsync(Models.Program program);
        Task<bool> DeleteProgramAsync(int id);
    }
}
