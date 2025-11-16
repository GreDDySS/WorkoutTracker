using Microsoft.EntityFrameworkCore;
using System.IO;
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
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "workout.db");
            
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Exercise>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.WorkTimeSeconds).IsRequired();
                entity.Property(e => e.RestTimeSeconds).IsRequired();
                entity.Property(e => e.IsCustom).IsRequired();
                entity.Property(e => e.Approaches).HasDefaultValue(1);
            });

            modelBuilder.Entity<Program>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.HasMany(p => p.Exercises)
                      .WithOne()
                      .HasForeignKey(pe => pe.ProgramId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProgramExercise>(entity =>
            {
                entity.HasKey(pe => pe.Id);
                entity.Property(pe => pe.Id).ValueGeneratedOnAdd();
                entity.Property(pe => pe.ProgramId).IsRequired();
                entity.Property(pe => pe.ExerciseId).IsRequired();
                entity.Property(pe => pe.Approaches).IsRequired().HasDefaultValue(1);

                entity.HasOne(pe => pe.Exercise)
                      .WithMany()
                      .HasForeignKey(pe => pe.ExerciseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Exercise>()
                .HasIndex(e => e.IsCustom);

            modelBuilder.Entity<ProgramExercise>()
                .HasIndex(pe => pe.ProgramId);

            modelBuilder.Entity<ProgramExercise>()
                .HasIndex(pe => pe.ExerciseId);

            modelBuilder.Entity<WorkoutHistory>(entity =>
            {
                entity.HasKey(wh => wh.Id);
                entity.Property(wh => wh.Id).ValueGeneratedOnAdd();
                entity.Property(wh => wh.WorkoutDate).IsRequired();
                entity.Property(wh => wh.WorkoutName).HasMaxLength(200);
                entity.Property(wh => wh.ProgramName).HasMaxLength(200);
                entity.Property(wh => wh.WorkoutDetails).HasMaxLength(2000);
            });

            modelBuilder.Entity<WorkoutHistory>()
                .HasIndex(wh => wh.WorkoutDate);
        }

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                // Проверяем, существует ли таблица WorkoutHistory
                var canConnect = await Database.CanConnectAsync();
                if (canConnect)
                {
                    // Проверяем, есть ли таблица WorkoutHistory
                    try
                    {
                        await Database.ExecuteSqlRawAsync("SELECT COUNT(*) FROM WorkoutHistories LIMIT 1");
                    }
                    catch
                    {
                        // Таблицы нет, нужно обновить схему
                        // Для SQLite проще всего пересоздать базу, но это удалит данные
                        // Вместо этого создадим таблицу вручную, если её нет
                        await Database.ExecuteSqlRawAsync(@"
                            CREATE TABLE IF NOT EXISTS WorkoutHistories (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                WorkoutDate TEXT NOT NULL,
                                WorkoutName TEXT,
                                TotalDurationSeconds INTEGER NOT NULL,
                                IsProgram INTEGER NOT NULL,
                                ProgramId INTEGER,
                                ProgramName TEXT,
                                WorkoutDetails TEXT
                            )");
                        await Database.ExecuteSqlRawAsync("CREATE INDEX IF NOT EXISTS IX_WorkoutHistories_WorkoutDate ON WorkoutHistories(WorkoutDate)");
                    }
                }
                else
                {
                    await Database.EnsureCreatedAsync();
                }
            }
            catch
            {
                // Если что-то пошло не так, просто создаем базу заново
                await Database.EnsureCreatedAsync();
            }

            if (!Exercises.Any(e => !e.IsCustom))
            {
                var systemExercises = new List<Exercise>
                {
                    new Exercise { Name = "Отжимания", WorkTimeSeconds = 20, RestTimeSeconds = 10, IsCustom = false, Approaches = 1 },
                    new Exercise { Name = "Приседания", WorkTimeSeconds = 25, RestTimeSeconds = 15, IsCustom = false, Approaches = 1 },
                    new Exercise { Name = "Планка", WorkTimeSeconds = 30, RestTimeSeconds = 10, IsCustom = false, Approaches = 1 },
                    new Exercise { Name = "Берпи", WorkTimeSeconds = 20, RestTimeSeconds = 15, IsCustom = false, Approaches = 1 },
                    new Exercise { Name = "Скручивания", WorkTimeSeconds = 25, RestTimeSeconds = 10, IsCustom = false, Approaches = 1 }
                };

                Exercises.AddRange(systemExercises);
                await SaveChangesAsync();
            }
        }
    }
}
