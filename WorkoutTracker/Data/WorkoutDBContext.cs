using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models;
using System.IO;

namespace WorkoutTracker.Data
{
    public class WorkoutDbContext : DbContext
    {
        public DbSet<WorkoutTimer> Timers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "workout.db");
            
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
    }
}
