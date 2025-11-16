using System.Windows.Input;
using WorkoutTracker.Base;
using WorkoutTracker.Services;

namespace WorkoutTracker.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;

        private int _defaultWorkTimeSeconds;
        private int _defaultRestTimeSeconds;
        private int _defaultApproaches;
        private bool _soundEnabled;
        private bool _vibrationEnabled;
        private bool _autoStartNextExercise;
        private int _countdownWarningSeconds;

        public int DefaultWorkTimeSeconds
        {
            get => _defaultWorkTimeSeconds;
            set
            {
                if (SetProperty(ref _defaultWorkTimeSeconds, value))
                {
                    _settingsService.DefaultWorkTimeSeconds = value;
                    OnPropertyChanged(nameof(WorkTimeDisplay));
                }
            }
        }

        public int DefaultRestTimeSeconds
        {
            get => _defaultRestTimeSeconds;
            set
            {
                if (SetProperty(ref _defaultRestTimeSeconds, value))
                {
                    _settingsService.DefaultRestTimeSeconds = value;
                    OnPropertyChanged(nameof(RestTimeDisplay));
                }
            }
        }

        public int DefaultApproaches
        {
            get => _defaultApproaches;
            set
            {
                if (SetProperty(ref _defaultApproaches, value))
                {
                    _settingsService.DefaultApproaches = value;
                }
            }
        }

        public bool SoundEnabled
        {
            get => _soundEnabled;
            set
            {
                if (SetProperty(ref _soundEnabled, value))
                {
                    _settingsService.SoundEnabled = value;
                }
            }
        }

        public bool VibrationEnabled
        {
            get => _vibrationEnabled;
            set
            {
                if (SetProperty(ref _vibrationEnabled, value))
                {
                    _settingsService.VibrationEnabled = value;
                }
            }
        }

        public bool AutoStartNextExercise
        {
            get => _autoStartNextExercise;
            set
            {
                if (SetProperty(ref _autoStartNextExercise, value))
                {
                    _settingsService.AutoStartNextExercise = value;
                }
            }
        }

        public int CountdownWarningSeconds
        {
            get => _countdownWarningSeconds;
            set
            {
                if (SetProperty(ref _countdownWarningSeconds, value))
                {
                    _settingsService.CountdownWarningSeconds = value;
                }
            }
        }

        public string WorkTimeDisplay => TimeFormatter.FormatTime(DefaultWorkTimeSeconds);
        public string RestTimeDisplay => TimeFormatter.FormatTime(DefaultRestTimeSeconds);

        public ICommand ChangeWorkTimeCommand { get; }
        public ICommand ChangeRestTimeCommand { get; }
        public ICommand ChangeApproachesCommand { get; }
        public ICommand ChangeWarningCommand { get; }

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            LoadSettings();

            ChangeWorkTimeCommand = new RelayCommand(OnChangeWorkTime);
            ChangeRestTimeCommand = new RelayCommand(OnChangeRestTime);
            ChangeApproachesCommand = new RelayCommand(OnChangeApproaches);
            ChangeWarningCommand = new RelayCommand(OnChangeWarning);
        }

        private void LoadSettings()
        {
            _defaultWorkTimeSeconds = _settingsService.DefaultWorkTimeSeconds;
            _defaultRestTimeSeconds = _settingsService.DefaultRestTimeSeconds;
            _defaultApproaches = _settingsService.DefaultApproaches;
            _soundEnabled = _settingsService.SoundEnabled;
            _vibrationEnabled = _settingsService.VibrationEnabled;
            _autoStartNextExercise = _settingsService.AutoStartNextExercise;
            _countdownWarningSeconds = _settingsService.CountdownWarningSeconds;

            OnPropertyChanged(nameof(DefaultWorkTimeSeconds));
            OnPropertyChanged(nameof(DefaultRestTimeSeconds));
            OnPropertyChanged(nameof(DefaultApproaches));
            OnPropertyChanged(nameof(SoundEnabled));
            OnPropertyChanged(nameof(VibrationEnabled));
            OnPropertyChanged(nameof(AutoStartNextExercise));
            OnPropertyChanged(nameof(CountdownWarningSeconds));
            OnPropertyChanged(nameof(WorkTimeDisplay));
            OnPropertyChanged(nameof(RestTimeDisplay));
        }

        private void OnChangeWorkTime(object parameter)
        {
            if (parameter is string direction)
            {
                if (direction == "+")
                {
                    DefaultWorkTimeSeconds = Math.Min(DefaultWorkTimeSeconds + 5, 300);
                }
                else if (direction == "-")
                {
                    DefaultWorkTimeSeconds = Math.Max(DefaultWorkTimeSeconds - 5, 5);
                }
            }
        }

        private void OnChangeRestTime(object parameter)
        {
            if (parameter is string direction)
            {
                if (direction == "+")
                {
                    DefaultRestTimeSeconds = Math.Min(DefaultRestTimeSeconds + 5, 300);
                }
                else if (direction == "-")
                {
                    DefaultRestTimeSeconds = Math.Max(DefaultRestTimeSeconds - 5, 5);
                }
            }
        }

        private void OnChangeApproaches(object parameter)
        {
            if (parameter is string direction)
            {
                if (direction == "+")
                {
                    DefaultApproaches = Math.Min(DefaultApproaches + 1, 99);
                }
                else if (direction == "-")
                {
                    DefaultApproaches = Math.Max(DefaultApproaches - 1, 1);
                }
            }
        }

        private void OnChangeWarning(object parameter)
        {
            if (parameter is string direction)
            {
                if (direction == "+")
                {
                    CountdownWarningSeconds = Math.Min(CountdownWarningSeconds + 1, 10);
                }
                else if (direction == "-")
                {
                    CountdownWarningSeconds = Math.Max(CountdownWarningSeconds - 1, 0);
                }
            }
        }
    }
}
