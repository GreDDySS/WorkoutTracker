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
        private bool _isDisposed = false;

        private string _remainingTime = "00:00";
        private string _currentTime = "00:00";
        private string _workoutProgress = "";

        public TimerViewModel(
            IWorkoutTimerService timerService,
            IWorkoutStateService stateService,
            INavigationService navigationService,
            WorkoutSettings settings)
        {
            _timerService = timerService;
            _stateService = stateService;
            _navigationService = navigationService;

            _stateService.Initialize(settings);

            _timerService.TimeElapsed += OnTimeElapsed;
            _timerService.PhaseCompleted += OnPhaseCompleted;

            UpdateDisplay();
            _timerService.Start();

            CloseCommand = new RelayCommand(OnClose);
            PreviousCommand = new RelayCommand(OnPrevious);
            PauseCommand = new RelayCommand(OnPause);
            NextCommand = new RelayCommand(OnNext);
        }


        private void OnTimeElapsed(object sender, int seconds)
        {
            if (_isDisposed) return;

            if (!_stateService.CurrentState.IsPaused && !_stateService.IsWorkoutCompleted())
            {
                _stateService.DecrementTime();

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

        private void OnPhaseCompleted(object sender, EventArgs e)
        {
            if (_isDisposed) return;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (!_isDisposed)
                {
                    UpdateDisplay();
                }
            });
        }

        private void UpdateDisplay()
        {
            if (_isDisposed) return;
            CurrentTime = TimeFormatter.FormatTime(_stateService.CurrentState.CurrentTimeSeconds);
            RemainingTime = TimeFormatter.FormatTime(_stateService.CalculateRemainingTime());
            WorkoutProgress = _stateService.GetProgressText();
        }

        public bool IsPaused
        {
            get => _stateService.CurrentState.IsPaused;
            set
            {
                if (_isDisposed) return;
                _stateService.CurrentState.IsPaused = value;
                if (value)
                    _timerService.Pause();
                else
                    _timerService.Resume();
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



        public ICommand CloseCommand { get; }
        public ICommand ChatCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand NextCommand { get; }


        private async void OnClose(object parameter)
        {
            if (_isDisposed) return;
            _timerService.Stop();
            await _navigationService.NavigateBackAsync();
        }

        private void OnPause(object parameter)
        {
            if (_isDisposed) return;
            IsPaused = !IsPaused;
        }

        private void OnNext(object parameter)
        {
            if (_isDisposed) return;
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
            }
            else
            {
                UpdateDisplay();
            }
        }

        private void OnPrevious(object parameter)
        {
            if (_isDisposed) return;
            _stateService.MoveToPreviousPhase();
            UpdateDisplay();
        }

        private async Task OnWorkoutCompleted()
        {
            if (_isDisposed) return;
            _timerService.Stop();

            try
            {
                await _navigationService.ShowWorkoutCompletedAlertAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при показе алерта: {ex.Message}");
            }

            try
            {
                await _navigationService.NavigateBackAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при навигации: {ex.Message}");
            }
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
