using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Services
{
    public class WorkoutHistoryService : IWorkoutHistoryService
    {
        private readonly WorkoutDbContext _context;

        public WorkoutHistoryService(WorkoutDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveWorkoutAsync(WorkoutHistory workout)
        {
            try
            {
                _context.WorkoutHistories.Add(workout);
                await _context.SaveChangesAsync();
                return workout.Id;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при сохранении тренировки: {ex.Message}");
                return 0;
            }
        }

        public async Task<List<WorkoutHistory>> GetWorkoutsByDateAsync(DateTime date)
        {
            try
            {
                var startDate = date.Date;
                var endDate = startDate.AddDays(1);
                
                return await _context.WorkoutHistories
                    .Where(wh => wh.WorkoutDate >= startDate && wh.WorkoutDate < endDate)
                    .OrderByDescending(wh => wh.WorkoutDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке тренировок: {ex.Message}");
                return new List<WorkoutHistory>();
            }
        }

        public async Task<List<WorkoutHistory>> GetWorkoutsByMonthAsync(int year, int month)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1);
                
                return await _context.WorkoutHistories
                    .Where(wh => wh.WorkoutDate >= startDate && wh.WorkoutDate < endDate)
                    .OrderByDescending(wh => wh.WorkoutDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке тренировок за месяц: {ex.Message}");
                return new List<WorkoutHistory>();
            }
        }

        public async Task<Dictionary<DateTime, int>> GetWorkoutDaysInMonthAsync(int year, int month)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1);
                
                var workouts = await _context.WorkoutHistories
                    .Where(wh => wh.WorkoutDate >= startDate && wh.WorkoutDate < endDate)
                    .ToListAsync();

                return workouts
                    .GroupBy(wh => wh.WorkoutDate.Date)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при получении дней с тренировками: {ex.Message}");
                return new Dictionary<DateTime, int>();
            }
        }
    }
}

