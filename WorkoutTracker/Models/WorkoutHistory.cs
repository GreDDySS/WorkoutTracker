namespace WorkoutTracker.Models
{
    public class WorkoutHistory
    {
        public int Id { get; set; }
        public DateTime WorkoutDate { get; set; }
        public string WorkoutName { get; set; }
        public int TotalDurationSeconds { get; set; }
        public bool IsProgram { get; set; }
        public int? ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string WorkoutDetails { get; set; } // JSON или строка с деталями тренировки
    }
}
