using System.Windows.Input;
using WorkoutTracker.Services;
using WorkoutTracker.Base;
using WorkoutTracker.Models;

namespace WorkoutTracker.ViewModels
{
    public class AddExerciseViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IExerciseService _exerciseService;
        private readonly ITimeAdjustmentService _timeAdjustmentService;
        public RelayCommand _saveCommand;

        private string _exerciseName = string.Empty;
        private int _workTimeSeconds = 20;
        private int _restTimeSeconds = 10;
        private int _exerciseId = 0;
        private bool _isEditMode = false;

        public string ExerciseName
        {
            get => _exerciseName;
            set
            {
                if (SetProperty(ref _exerciseName, value))
                {
                    _saveCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public int WorkTimeSeconds
        {
            get => _workTimeSeconds;
            set
            {
                if (SetProperty(ref _workTimeSeconds, value))
                {
                    OnPropertyChanged(nameof(WorkTimeDisplay));
                    _saveCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public int RestTimeSeconds
        {
            get => _restTimeSeconds;
            set
            {
                if (SetProperty(ref _restTimeSeconds, value))
                {
                    OnPropertyChanged(nameof(RestTimeDisplay));
                    _saveCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public int ExerciseId
        {
            get => _exerciseId;
            set => SetProperty(ref _exerciseId, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public string WorkTimeDisplay => TimeFormatter.FormatTime(WorkTimeSeconds);
        public string RestTimeDisplay => TimeFormatter.FormatTime(RestTimeSeconds);
        public string Title => IsEditMode ? "РЕДАКТИРОВАТЬ УПРАЖНЕНИЕ" : "НОВОЕ УПРАЖНЕНИЕ";

        public ICommand CloseCommand { get; }
        public ICommand SaveCommand => _saveCommand;
        public ICommand ChangeWorkTimeCommand { get; }
        public ICommand ChangeRestTimeCommand { get; }

        public AddExerciseViewModel()
        {
            _navigationService = GetService<INavigationService>() ?? new NavigationService();
            _dialogService = GetService<IDialogService>() ?? new DialogService();
            _exerciseService = GetService<IExerciseService>() ?? new ExerciseService(null);
            _timeAdjustmentService = GetService<ITimeAdjustmentService>() ?? new TimeAdjustmentService();

            CloseCommand = new RelayCommand(OnClose);
            _saveCommand = new RelayCommand(OnSave, CanSave);
            ChangeWorkTimeCommand = new RelayCommand(OnChangeWorkTime);
            ChangeRestTimeCommand = new RelayCommand(OnChangeRestTime);
        }

        private bool CanSave(object parameter)
        {
            return !string.IsNullOrWhiteSpace(ExerciseName) &&
                   WorkTimeSeconds > 0 &&
                   RestTimeSeconds >= 0;
        }

        private void OnChangeWorkTime(object parameter)
        {
            if (parameter is string direction)
            {
                WorkTimeSeconds = _timeAdjustmentService.AdjustTime(WorkTimeSeconds, direction, 5, 5);
            }
        }

        private void OnChangeRestTime(object parameter)
        {
            if (parameter is string direction)
            {
                RestTimeSeconds = _timeAdjustmentService.AdjustTime(RestTimeSeconds, direction, 5, 5);
            }
        }

        private async void OnClose(object parameter)
        {
            await _navigationService.NavigateBackAsync();
        }

        private async void OnSave(object parameter)
        {
            if (!CanSave(null))
            {
                await _dialogService.ShowAlertAsync("Ошибка", "Пожалуйста, заполните все поля корректно");
                return;
            }

            var exercise = new Exercise
            {
                Id = ExerciseId,
                Name = ExerciseName,
                WorkTimeSeconds = WorkTimeSeconds,
                RestTimeSeconds = RestTimeSeconds,
                IsCustom = true
            };

            await _exerciseService.SaveExerciseAsync(exercise);
            await _navigationService.NavigateBackAsync();
        }

        public async Task LoadExerciseAsync(int exerciseId)
        {
            try
            {
                ExerciseId = exerciseId;
                IsEditMode = true;
                OnPropertyChanged(nameof(IsEditMode));
                OnPropertyChanged(nameof(Title));

                var exercise = await _exerciseService.GetExerciseByIdAsync(exerciseId);
                if (exercise != null)
                {
                    ExerciseName = exercise.Name;
                    WorkTimeSeconds = exercise.WorkTimeSeconds;
                    RestTimeSeconds = exercise.RestTimeSeconds;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке упражнения: {ex.Message}");
            }
        }

        public void ResetState()
        {
            ExerciseId = 0;
            IsEditMode = false;
            ExerciseName = string.Empty;
            WorkTimeSeconds = 20;
            RestTimeSeconds = 10;
            OnPropertyChanged(nameof(IsEditMode));
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(WorkTimeDisplay));
            OnPropertyChanged(nameof(RestTimeDisplay));
        }
    }
}
