using Microsoft.Extensions.Logging;
using WorkoutTracker.Data;
using WorkoutTracker.Services;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<WorkoutDbContext>();

            builder.Services.AddSingleton<IWorkoutTimerService, WorkoutTimerService>();
            builder.Services.AddTransient<IWorkoutStateService, WorkoutStateService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<IExerciseService, ExerciseService>();
            builder.Services.AddSingleton<IProgramService, ProgramService>();
            builder.Services.AddSingleton<ITimeAdjustmentService, TimeAdjustmentService>();

            builder.Services.AddTransient<TimerViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
