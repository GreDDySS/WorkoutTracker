using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutTracker.Models
{
    [Table("Exercises")]
    [Index(nameof(IsCustom))]
    public class Exercise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [Required]
        [Range(1, 3600)]
        public int WorkTimeSeconds { get; set; }
        [Required]
        [Range(1, 3600)]
        public int RestTimeSeconds { get; set; }
        [Required]
        public bool IsCustom { get; set; }
        public int Approaches { get; set; } = 1;
    }
}
