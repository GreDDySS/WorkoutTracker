using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models
{
    public class WorkoutTimer
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public int Sets { get; set; }
        public int WorkDuration { get; set; }
        public int RestInterval { get; set; }
    }
}
