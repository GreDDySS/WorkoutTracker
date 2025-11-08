using Microsoft.EntityFrameworkCore;
using System.IO;

namespace WorkoutTracker.Data
{
    public class WorkoutDbContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "workout.db");
            
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
    }
}
