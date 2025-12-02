using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Services
{
    public class WorkoutHistoryService : IWorkoutHistoryService
    {
        private readonly WorkoutDbContext _context;

        public WorkoutHistoryService(WorkoutDbContext context) => _context = context;

        public async Task<int> SaveWorkoutAsync(WorkoutHistory workout)
        {
            if (workout == null) return 0;

            workout.WorkoutDate = workout.WorkoutDate == default(DateTime) ? DateTime.Now : workout.WorkoutDate;

            try
            {
                _context.WorkoutHistories.Add(workout);
                await _context.SaveChangesAsync();
                return workout.Id;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WorkoutHistoryService] SaveWorkoutAsync ошибка: {ex.InnerException?.Message}\n{ex.StackTrace}");
                return 0;
            }

        }

        public async Task<List<WorkoutHistory>> GetWorkoutsByDateAsync(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            return await _context.WorkoutHistories
                .Where(wh => wh.WorkoutDate >= start && wh.WorkoutDate < end)
                .OrderByDescending(wh => wh.WorkoutDate)
                .ToListAsync();
        }

        public async Task<List<WorkoutHistory>> GetWorkoutsByMonthAsync(int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);

            return await _context.WorkoutHistories
                .Where(wh => wh.WorkoutDate >= start && wh.WorkoutDate < end)
                .OrderByDescending(wh => wh.WorkoutDate)
                .ToListAsync();
        }

        public async Task<Dictionary<DateTime, int>> GetWorkoutDaysInMonthAsync(int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);

            return await _context.WorkoutHistories
                .Where(wh => wh.WorkoutDate >= start && wh.WorkoutDate < end)
                .GroupBy(wh => wh.WorkoutDate.Date)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }
    }
}

