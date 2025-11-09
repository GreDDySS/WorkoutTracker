namespace WorkoutTracker.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WorkTimeSeconds { get; set; }
        public int RestTimeSeconds { get; set; }
        public bool IsCustom { get; set; }
        public int Approaches { get; set; } = 1;
    }
}
