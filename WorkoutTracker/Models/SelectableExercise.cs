using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Models
{
    public class SelectableExercise : BaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WorkTimeSeconds { get; set; }
        public int RestTimeSeconds { get; set; }
        public bool IsCustom { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public string Details => $"Работа: {WorkTimeSeconds}с, Отдых: {RestTimeSeconds}с";
    }
}
