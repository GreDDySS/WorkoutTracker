using System.Collections.ObjectModel;
using System.Windows.Input;
using WorkoutTracker.Base;
using WorkoutTracker.Models;
using WorkoutTracker.Services;

namespace WorkoutTracker.ViewModels
{
    public class SelectExercisesViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IExerciseService _exerciseService;

        public ObservableCollection<SelectableExercise> CustomExercises { get; set; }
        public ObservableCollection<SelectableExercise> SystemExercises { get; set; }

        public bool HasNoCustomExercises => CustomExercises?.Count == 0;
        public bool HasNoSystemExercises => SystemExercises?.Count == 0;

        public int SelectedCount =>
            (CustomExercises?.Count(e => e.IsSelected) ?? 0) +
            (SystemExercises?.Count(e => e.IsSelected) ?? 0);

        public bool HasSelectedExercises => SelectedCount > 0;
        public string AddButtonText => SelectedCount > 0
            ? $"Добавить {SelectedCount} упражнений"
            : "Добавить упражнения";

        public event EventHandler<List<SelectableExercise>> ExercisesSelected;

        public ICommand CloseCommand { get; }
        public ICommand AddSelectedCommand { get; }

        public SelectExercisesViewModel()
        {
            _navigationService = GetService<INavigationService>() ?? new NavigationService();
            _exerciseService = GetService<IExerciseService>() ?? new ExerciseService(null);

            CustomExercises = new ObservableCollection<SelectableExercise>();
            SystemExercises = new ObservableCollection<SelectableExercise>();

            CustomExercises.CollectionChanged += (s, e) => UpdateSelectedCount();
            SystemExercises.CollectionChanged += (s, e) => UpdateSelectedCount();

            foreach (var exercise in CustomExercises)
            {
                exercise.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(SelectableExercise.IsSelected)) UpdateSelectedCount(); };
            }
            foreach (var exercise in SystemExercises)
            {
                exercise.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(SelectableExercise.IsSelected)) UpdateSelectedCount(); };
            }

            LoadExercises();

            CloseCommand = new RelayCommand(OnClose);
            AddSelectedCommand = new RelayCommand(OnAddSelected, (object _) => HasSelectedExercises);
        }

        public void UpdateSelectedCount()
        {
            OnPropertyChanged(nameof(SelectedCount));
            OnPropertyChanged(nameof(HasSelectedExercises));
            OnPropertyChanged(nameof(AddButtonText));
        }

        private async void LoadExercises()
        {
            var customExercises = await _exerciseService.GetCustomExercisesAsync();
            var systemExercises = await _exerciseService.GetSystemExercisesAsync();

            foreach(var ex in customExercises)
            {
                CustomExercises.Add(new SelectableExercise
                {
                    Id = ex.Id,
                    Name = ex.Name,
                    WorkTimeSeconds = ex.WorkTimeSeconds,
                    RestTimeSeconds = ex.RestTimeSeconds,
                    IsCustom = true
                });
            }

            foreach(var ex in systemExercises)
            {
                SystemExercises.Add(new SelectableExercise
                {
                    Id = ex.Id,
                    Name = ex.Name,
                    WorkTimeSeconds = ex.WorkTimeSeconds,
                    RestTimeSeconds = ex.RestTimeSeconds,
                    IsCustom = false
                });
            }
        }

        private async void OnClose(object parameter)
        {
            await _navigationService.NavigateBackAsync();
        }

        private async void OnAddSelected(object parameter)
        {
            var selected = CustomExercises.Where(e => e.IsSelected)
                .Concat(SystemExercises.Where(e => e.IsSelected))
                .ToList();

            ExercisesSelected?.Invoke(this, selected);
            await _navigationService.NavigateBackAsync();
        }
    }
}
