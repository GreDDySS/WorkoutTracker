using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models;

namespace WorkoutTracker.Data
{
    public class WorkoutDbContext : DbContext
    {
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<ProgramExercise> ProgramExercises { get; set; }
        public DbSet<WorkoutHistory> WorkoutHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "workout.db");
            
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Exercise>()
                .Property(e => e.Approaches)
                .HasDefaultValue(1);

            modelBuilder.Entity<ProgramExercise>()
                .Property(pe => pe.Approaches)
                .HasDefaultValue(1);

            modelBuilder.Entity<ProgramExercise>(entity =>
            {
                entity.HasOne(pe => pe.Exercise)
                      .WithMany()
                      .HasForeignKey(pe => pe.ExerciseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public async Task InitializeDatabaseAsync()
        {
            await Database.EnsureCreatedAsync();

            if (!await Exercises.AnyAsync(e => !e.IsCustom))
            {
                var systemExercises = new List<Exercise>
                {
                    new Exercise { Id = -1, Name = "Отжимания",     WorkTimeSeconds = 20, RestTimeSeconds = 10, IsCustom = false, Approaches = 1 },
                    new Exercise { Id = -2, Name = "Приседания",   WorkTimeSeconds = 25, RestTimeSeconds = 15, IsCustom = false, Approaches = 1 },
                    new Exercise { Id = -3, Name = "Планка",       WorkTimeSeconds = 30, RestTimeSeconds = 10, IsCustom = false, Approaches = 1 },
                    new Exercise { Id = -4, Name = "Берпи",        WorkTimeSeconds = 20, RestTimeSeconds = 15, IsCustom = false, Approaches = 1 },
                    new Exercise { Id = -5, Name = "Скручивания",  WorkTimeSeconds = 25, RestTimeSeconds = 10, IsCustom = false, Approaches = 1 }
                };

                Exercises.AddRange(systemExercises);
                await SaveChangesAsync();
            }
        }
    }
}
