using WorkoutTracker.Services;
namespace WorkoutTracker.Models
{
    public class WorkoutSettings
    {
        public int Approaches { get; set; } = 1;
        public int WorkTimeSeconds { get; set; } = 20;
        public int RestTimeSeconds { get; set; } = 10;
        public List<ProgramExerciseItem> ProgramExercises { get; set; } = null;

        public bool IsProgram => ProgramExercises != null && ProgramExercises.Count > 0;

        public string WorkTimeDisplay => TimeFormatter.FormatTime(WorkTimeSeconds);
        public string RestTimeDisplay => TimeFormatter.FormatTime(RestTimeSeconds);

    }
}

