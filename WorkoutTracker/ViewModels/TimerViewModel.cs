using System.Linq;
using System.Windows.Input;
using WorkoutTracker.Base;
using WorkoutTracker.Models;
using WorkoutTracker.Services;

namespace WorkoutTracker.ViewModels
{
    public class TimerViewModel : BaseViewModel, IDisposable
    {
        private readonly IWorkoutTimerService _timerService;
        private readonly IWorkoutStateService _stateService;
        private readonly INavigationService _navigationService;
        private readonly IWorkoutHistoryService _historyService;
        private readonly ISettingsService _settingsService;
        private readonly IWorkoutNotificationService _notificationService;
        private readonly WorkoutSettings _settings;

        private bool _isDisposed = false;
        private readonly int _initialTotalTime;
        private readonly int _countdownWarningSeconds;

        private string _remainingTime = "00:00";
        private string _currentTime = "00:00";
        private string _workoutProgress = "";
        private int? _lastCountdownAlertSecond = null;

        public TimerViewModel(
            IWorkoutTimerService timerService, 
            IWorkoutStateService stateService, 
            INavigationService navigationService, 
            IWorkoutHistoryService historyService, 
            ISettingsService settingsService, 
            IWorkoutNotificationService notificationService, 
            WorkoutSettings settings)
        {
            _timerService = timerService;
            _stateService = stateService;
            _navigationService = navigationService;
            _historyService = historyService;
            _settingsService = settingsService;
            _notificationService = notificationService;
            _settings = settings;
            _countdownWarningSeconds = _settingsService?.CountdownWarningSeconds ?? 0;

            _stateService.Initialize(settings);
            _initialTotalTime = _stateService.CalculateRemainingTime();

            _timerService.TimeElapsed += OnTimeElapsed;
            _timerService.PhaseCompleted += OnPhaseCompleted;

            UpdateDisplay();
            _timerService.Start();

            CloseCommand = new RelayCommand(_ => OnClose());
            PauseCommand = new RelayCommand(_ => IsPaused = !IsPaused);
            NextCommand = new RelayCommand(_ => OnNext());
            PreviousCommand = new RelayCommand(_ => OnPrevious());
        }


        private void OnTimeElapsed(object? sender, int seconds)
        {
            if (_isDisposed) return;

            if (!_stateService.CurrentState.IsPaused && !_stateService.IsWorkoutCompleted())
            {
                _stateService.DecrementTime();
                HandleCountdownWarning();

                if (_stateService.CurrentState.CurrentTimeSeconds == 0)
                {
                    _stateService.MoveToNextPhase();
                    if (_stateService.IsWorkoutCompleted())
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            if (!_isDisposed)
                            {
                                await OnWorkoutCompleted();
                            }
                        });
                        return;
                    }
                    ResetCountdownTracking();
                    TriggerPhaseChangeNotification();
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (!_isDisposed)
                    {
                        UpdateDisplay();
                    }
                });
            }
        }

        private void OnPhaseCompleted(object? sender, EventArgs e)
        {
            if (_isDisposed) return;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (!_isDisposed)
                {
                    UpdateDisplay();
                    OnPhaseChanged();
                }
            });
        }

        private void UpdateDisplay()
        {
            if (_isDisposed) return;

            CurrentTime = TimeFormatter.FormatTime(_stateService.CurrentState.CurrentTimeSeconds);
            RemainingTime = TimeFormatter.FormatTime(_stateService.CalculateRemainingTime());
            WorkoutProgress = _stateService.GetProgressText();
            OnPropertyChanged(nameof(IsWorkPhase));
        }

        public bool IsPaused
        {
            get => _stateService.CurrentState.IsPaused;
            set
            {
                if (_isDisposed) return;

                _stateService.CurrentState.IsPaused = value;
                if (value) _timerService.Pause();
                else _timerService.Resume();

                OnPropertyChanged(nameof(IsPaused));
                OnPropertyChanged(nameof(PauseButtonFullText));
            }
        }

        public string PauseButtonText => IsPaused ? "Продолжить" : "Пауза";
        public string PauseButtonFullText => $"⏸ {PauseButtonText}";


        public string RemainingTime
        {
            get => _remainingTime;
            set => SetProperty(ref _remainingTime, value);
        }

        public string CurrentTime
        {
            get => _currentTime;
            set => SetProperty(ref _currentTime, value);
        }

        public string WorkoutProgress
        {
            get => _workoutProgress;
            set => SetProperty(ref _workoutProgress, value);
        }

        public bool IsWorkPhase => _stateService.CurrentState.IsWorkPhase;



        public ICommand CloseCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand NextCommand { get; }


        private void OnClose()
        {
            if (_isDisposed) return;
            _timerService.Stop();
            _ = _navigationService.NavigateBackAsync();
        }

        private void OnPause()
        {
            if (_isDisposed) return;
            IsPaused = !IsPaused;
        }

        private void OnNext()
        {
            if (_isDisposed) return;
            _stateService.MoveToNextPhase();
            if (_stateService.IsWorkoutCompleted())
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await OnWorkoutCompleted();
                });
            }
            else
            {
                ResetCountdownTracking();
                TriggerPhaseChangeNotification();
                UpdateDisplay();
                OnPhaseChanged();
            }
        }

        private void OnPrevious()
        {
            if (_isDisposed) return;
            _stateService.MoveToPreviousPhase();
            ResetCountdownTracking();
            TriggerPhaseChangeNotification();
            UpdateDisplay();
            OnPhaseChanged();
        }

        private void OnPhaseChanged()
        {
            if (_isDisposed) return;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (!_isDisposed)
                {
                    OnPropertyChanged(nameof(IsWorkPhase));
                }
            });
        }

        private void HandleCountdownWarning()
        {
            if (_notificationService == null || _countdownWarningSeconds <= 0) return;

            int secondsLeft = _stateService.CurrentState.CurrentTimeSeconds;

            if (secondsLeft > 0 && secondsLeft <= _countdownWarningSeconds)
            {
                if (_lastCountdownAlertSecond <= secondsLeft)
                {
                    _lastCountdownAlertSecond = secondsLeft;
                    TriggerCountdownNotification();
                }
            }
            else if (secondsLeft > _countdownWarningSeconds)
            {
                _lastCountdownAlertSecond = null;
            }
        }

        private void TriggerCountdownNotification()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (!_isDisposed)
                {
                    await _notificationService.PlayCountdownTickAsync();
                }
            });
        }

        private void TriggerPhaseChangeNotification()
        {
            if (_notificationService == null) return;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (!_isDisposed)
                {
                    await _notificationService.PlayPhaseChangeAsync(_stateService.CurrentState.IsWorkPhase);
                }
            });
        }

        private void ResetCountdownTracking()
        {
            _lastCountdownAlertSecond = null;
        }

        private async Task OnWorkoutCompleted()
        {
            if (_isDisposed) return;

            _timerService.Stop();

            var totalDuration = _initialTotalTime - _stateService.CalculateRemainingTime();

            var workoutName = _settings.IsProgram && _settings.ProgramExercises != null && _settings.ProgramExercises.Count > 0
                            ? _settings.ProgramExercises[0].ExerciseName + " и др."
                            : "Свободная тренировка";

            var details = _settings.IsProgram && _settings.ProgramExercises?.Any() == true
                ? string.Join(", ", _settings.ProgramExercises.Select(pe => $"{pe.ExerciseName} ({pe.Approaches})"))
                : $"{_settings.Approaches} подходов × {_settings.WorkTimeDisplay}/{_settings.RestTimeDisplay}";

            var history = new WorkoutHistory
            {
                WorkoutDate = DateTime.Now,
                WorkoutName = workoutName,
                TotalDurationSeconds = totalDuration,
                IsProgram = _settings.IsProgram,
                ProgramId = null,
                ProgramName = workoutName,
                WorkoutDetails = details
            };

            await _historyService.SaveWorkoutAsync(history);

            await _navigationService.ShowWorkoutCompletedAlertAsync();
            await _navigationService.NavigateBackAsync();
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            _timerService.Stop();
            _timerService.TimeElapsed -= OnTimeElapsed;
            _timerService.PhaseCompleted -= OnPhaseCompleted;
            _timerService?.Dispose();
        }


    }
}
