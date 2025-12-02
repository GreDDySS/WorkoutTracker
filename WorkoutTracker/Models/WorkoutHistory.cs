using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutTracker.Models
{
    [Table("WorkoutHistories")]
    [Index(nameof(WorkoutDate))]
    public class WorkoutHistory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime WorkoutDate { get; set; }
        [Required]
        public string WorkoutName { get; set; }
        [Required]
        public int TotalDurationSeconds { get; set; }
        [Required]
        public bool IsProgram { get; set; }
        public int? ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string WorkoutDetails { get; set; }
    }
}
