namespace WorkoutTracker.Models
{
    public class ProgramExercise
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }
        public int Approaches { get; set; }
    }
}
