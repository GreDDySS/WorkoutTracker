using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutTracker.Models
{
    [Table("ProgramExercises")]
    [Index(nameof(ProgramId))]
    [Index(nameof(ExerciseId))]
    public class ProgramExercise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int ProgramId { get; set; }
        [Required]
        public int ExerciseId { get; set; }

        [ForeignKey(nameof(ExerciseId))]
        public Exercise Exercise { get; set; } = null;
        [ForeignKey(nameof(ProgramId))]
        public Program Program { get; set; } = null;
        [Required]
        public int Approaches { get; set; } = 1;
    }
}
