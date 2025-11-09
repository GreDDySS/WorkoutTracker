namespace WorkoutTracker.Models
{
    public class Program
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProgramExercise> Exercises { get; set; } = new();
    }
}