using WorkoutTracker.Data;
using WorkoutTracker.Services;
using WorkoutTracker.ViewModels;
using WorkoutTracker.Views;
using Plugin.Maui.Audio;
using Microsoft.Extensions.Logging;

namespace WorkoutTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            SQLitePCL.Batteries.Init();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddDbContext<WorkoutDbContext>();
            builder.Services.AddSingleton<IWorkoutTimerService, WorkoutTimerService>();
            builder.Services.AddTransient<IWorkoutStateService, WorkoutStateService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<IExerciseService, ExerciseService>();
            builder.Services.AddSingleton<IProgramService, ProgramService>();
            builder.Services.AddSingleton<ITimeAdjustmentService, TimeAdjustmentService>();
            builder.Services.AddSingleton<IWorkoutHistoryService, WorkoutHistoryService>();
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddSingleton<IWorkoutNotificationService, WorkoutNotificationService>();

            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<StatsViewModel>();

            builder.Services.AddTransient<TimerPageView>();
            builder.Services.AddTransient<StatsPageView>();
            builder.Services.AddTransient<MainPageView>();
            builder.Services.AddTransient<SettingsPageView>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WorkoutDbContext>();
            db.InitializeDatabaseAsync().GetAwaiter().GetResult();

            return app;
        }
    }
}
