using System.Windows.Input;
using WorkoutTracker.Base;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Models
{
    public class ProgramExerciseItem : BaseViewModel
    {
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; }

        private int _approaches = 1;
        public int Approaches
        {
            get => _approaches;
            set
            {
                if (SetProperty(ref _approaches, value))
                {
                    OnPropertyChanged(nameof(ApproachesText));
                }
            }
        }

        public int WorkTimeSeconds { get; set; }
        public int RestTimeSeconds { get; set; }

        public string ApproachesText => $"{Approaches} Подход(ов)";

        public ICommand IncreaseApproachesCommand { get; }
        public ICommand DecreaseApproachesCommand { get; }

        public ProgramExerciseItem()
        {
            IncreaseApproachesCommand = new RelayCommand((_) => Approaches++);
            DecreaseApproachesCommand = new RelayCommand((_) => { if (Approaches > 1) Approaches--; });
        }

        public static ProgramExerciseItem FromSelectableExercise(SelectableExercise exercise)
        {
            return new ProgramExerciseItem
            {
                ExerciseId = exercise.Id,
                ExerciseName = exercise.Name,
                WorkTimeSeconds = exercise.WorkTimeSeconds,
                RestTimeSeconds = exercise.RestTimeSeconds,
                Approaches = 1 // По умолчанию 1 подход
            };
        }

        public ProgramExercise ToProgramExercise()
        {
            return new ProgramExercise
            {
                ExerciseId = ExerciseId,
                Approaches = Approaches
            };
        }
    }
}
