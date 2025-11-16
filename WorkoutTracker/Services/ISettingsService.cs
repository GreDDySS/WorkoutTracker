namespace WorkoutTracker.Services
{
    public interface ISettingsService
    {
        int DefaultWorkTimeSeconds { get; set; }
        int DefaultRestTimeSeconds { get; set; }
        int DefaultApproaches { get; set; }
        bool SoundEnabled { get; set; }
        bool VibrationEnabled { get; set; }
        bool AutoStartNextExercise { get; set; }
        int CountdownWarningSeconds { get; set; }
    }
}
