using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private RelayCommand _addSelectedCommand;

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
        public ICommand AddSelectedCommand => _addSelectedCommand;

        public SelectExercisesViewModel()
        {
            _navigationService = GetService<INavigationService>() ?? new NavigationService();
            _exerciseService = GetService<IExerciseService>() ?? new ExerciseService(null);

            CustomExercises = new ObservableCollection<SelectableExercise>();
            SystemExercises = new ObservableCollection<SelectableExercise>();

            CustomExercises.CollectionChanged += OnCollectionChanged;
            SystemExercises.CollectionChanged += OnCollectionChanged;


            CloseCommand = new RelayCommand(OnClose);
            _addSelectedCommand = new RelayCommand(OnAddSelected, CanAddSelected);

            LoadExercises();
        }

        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SelectableExercise item in e.NewItems)
                {
                    item.PropertyChanged += OnExercisePropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (SelectableExercise item in e.OldItems)
                {
                    item.PropertyChanged -= OnExercisePropertyChanged;
                }
            }
            UpdateSelectedCount();
        }

        private void OnExercisePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectableExercise.IsSelected))
            {
                UpdateSelectedCount();
            }
        }

        public void UpdateSelectedCount()
        {
            OnPropertyChanged(nameof(SelectedCount));
            OnPropertyChanged(nameof(HasSelectedExercises));
            OnPropertyChanged(nameof(AddButtonText));
        }

        private bool CanAddSelected(object parameter)
        {
            return HasSelectedExercises;
        }

        private async void LoadExercises()
        {
            try
            {
                var customExercises = await _exerciseService.GetCustomExercisesAsync();
                var systemExercises = await _exerciseService.GetSystemExercisesAsync();

                foreach (var ex in customExercises)
                {
                    var selectable = new SelectableExercise
                    {
                        Id = ex.Id,
                        Name = ex.Name,
                        WorkTimeSeconds = ex.WorkTimeSeconds,
                        RestTimeSeconds = ex.RestTimeSeconds,
                        IsCustom = true
                    };
                    selectable.PropertyChanged += OnExercisePropertyChanged;
                    CustomExercises.Add(selectable);
                }

                foreach (var ex in systemExercises)
                {
                    var selectable = new SelectableExercise
                    {
                        Id = ex.Id,
                        Name = ex.Name,
                        WorkTimeSeconds = ex.WorkTimeSeconds,
                        RestTimeSeconds = ex.RestTimeSeconds,
                        IsCustom = false
                    };
                    selectable.PropertyChanged += OnExercisePropertyChanged;
                    SystemExercises.Add(selectable);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке упражнений: {ex.Message}");
            }
        }

        private async void OnClose(object parameter)
        {
            await _navigationService.NavigateBackAsync();
        }

        private async void OnAddSelected(object parameter)
        {
            try
            {
                var selected = CustomExercises.Where(e => e.IsSelected)
                    .Concat(SystemExercises.Where(e => e.IsSelected))
                    .ToList();

                if (selected.Count > 0)
                {
                    ExercisesSelected?.Invoke(this, selected);
                }

                await _navigationService.NavigateBackAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при добавлении упражнений: {ex.Message}");
            }
        }
    }
}
