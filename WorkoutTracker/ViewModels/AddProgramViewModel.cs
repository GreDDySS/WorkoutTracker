using System.Collections.ObjectModel;
using System.Windows.Input;
using WorkoutTracker.Base;
using WorkoutTracker.Models;
using WorkoutTracker.Services;
using WorkoutTracker.Views;

namespace WorkoutTracker.ViewModels
{
    public class AddProgramViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IProgramService _programService;
        private RelayCommand _saveCommand;

        private string _programName = string.Empty;

        public string ProgramName
        {
            get => _programName;
            set
            {
                if (SetProperty(ref _programName, value))
                {
                    _saveCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<ProgramExerciseItem> ProgramExercises { get; set; }

        public bool HasExercises => ProgramExercises?.Count > 0;
        public bool HasNoExercises => !HasExercises;

        public ICommand CloseCommand { get; }
        public ICommand SaveCommand => _saveCommand;
        public ICommand AddExerciseCommand { get; }
        public ICommand RemoveExerciseCommand { get; }

        public AddProgramViewModel()
        {
            _navigationService = GetService<INavigationService>() ?? new NavigationService();
            _dialogService = GetService<IDialogService>() ?? new DialogService();
            _programService = GetService<IProgramService>() ?? new ProgramService(null);

            ProgramExercises = new ObservableCollection<ProgramExerciseItem>();

            ProgramExercises.CollectionChanged += OnProgramExercisesCollectionChanged;

            CloseCommand = new RelayCommand(OnClose);
            _saveCommand = new RelayCommand(OnSave, CanSave);
            AddExerciseCommand = new RelayCommand(OnAddExercise);
            RemoveExerciseCommand = new RelayCommand(OnRemoveExercise);
            
            _saveCommand?.RaiseCanExecuteChanged();
        }

        private void OnProgramExercisesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasExercises));
            OnPropertyChanged(nameof(HasNoExercises));
            _saveCommand?.RaiseCanExecuteChanged();
        }

        private bool CanSave(object obj)
        {
            return !string.IsNullOrWhiteSpace(ProgramName) && HasExercises;
        }

        private async void OnClose(object obj)
        {
            await _navigationService.NavigateBackAsync();
        }

        private async void OnSave(object obj)
        {
            if (!CanSave(null))
            {
                await _dialogService.ShowAlertAsync(
                    "Ошибка",
                    "Пожалуйста, заполните название программы и добавьте хотя бы одно упражнение");
                return;
            }

            var program = CreateProgramFromViewModel();
            await _programService.SaveProgramAsync(program);
            await _navigationService.NavigateBackAsync();
        }

        private Models.Program CreateProgramFromViewModel()
        {
            return new Models.Program
            {
                Name = ProgramName,
                Exercises = ProgramExercises
                    .Select(pe => pe.ToProgramExercise())
                    .ToList()
            };
        }

        private async void OnAddExercise(object parameter)
        {
            try
            {
                var selectPage = new SelectExercisesPageView();
                var selectViewModel = new SelectExercisesViewModel();
                selectPage.BindingContext = selectViewModel;

                selectViewModel.ExercisesSelected += OnExercisesSelected;

                await Shell.Current.Navigation.PushAsync(selectPage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при открытии страницы выбора: {ex.Message}");
                await _dialogService.ShowAlertAsync("Ошибка", "Не удалось открыть страницу выбора упражнений");
            }
        }

        private void OnExercisesSelected(object sender, List<SelectableExercise> selectedExercises)
        {
            if (selectedExercises == null || selectedExercises.Count == 0)
                return;

            foreach (var exercise in selectedExercises)
            {
                
                if (!ProgramExercises.Any(pe => pe.ExerciseId == exercise.Id))
                {
                    var programExercise = ProgramExerciseItem.FromSelectableExercise(exercise);
                    ProgramExercises.Add(programExercise);
                }
            }
        }

        private void OnRemoveExercise(object parameter)
        {
            if (parameter is ProgramExerciseItem exercise)
            {
                ProgramExercises.Remove(exercise);
            }
        }

    }
}
