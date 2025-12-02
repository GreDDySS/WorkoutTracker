using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using WorkoutTracker.Base;
using WorkoutTracker.Models;
using WorkoutTracker.Services;

namespace WorkoutTracker.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigationService;
        private WorkoutSettings _settings;

        public WorkoutSettings Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public int Approaches
        {
            get => _settings.Approaches;
            set
            {
                if (value > 0)
                {
                    _settings.Approaches = value;
                    OnPropertyChanged(nameof(Approaches));
                }
            }
        }

        public string WorkTimeDisplay => _settings.WorkTimeDisplay;
        public string RestTimeDisplay => _settings.RestTimeDisplay;

        public ICommand StartWorkoutCommand { get; }
        public ICommand ChangeApproachesCommand { get; }
        public ICommand ChangeWorkTimeCommand { get; }
        public ICommand ChangeRestTimeCommand { get; }

        public MainViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _navigationService = GetService<INavigationService>() ?? throw new Exception("INavigationService не зарегистрирован");

            _settings = new WorkoutSettings();
            RelaodSettingsFromPreferences();

            StartWorkoutCommand = new RelayCommand(OnStartWorkout);
            ChangeApproachesCommand = new RelayCommand(OnChangeApproaches);
            ChangeWorkTimeCommand = new RelayCommand(OnChangeWorkTime);
            ChangeRestTimeCommand = new RelayCommand(OnChangeRestTime);
        }

        public void RelaodSettingsFromPreferences()
        {
            Approaches = _settingsService.DefaultApproaches;
            _settings.WorkTimeSeconds = _settingsService.DefaultWorkTimeSeconds;
            _settings.RestTimeSeconds = _settingsService.DefaultRestTimeSeconds;
            OnPropertyChanged(nameof(WorkTimeDisplay));
            OnPropertyChanged(nameof(RestTimeDisplay));
        }

        private async void OnStartWorkout(object parameter)
        {
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync("TimerPage");
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void OnChangeApproaches(object parameter)
        {
            if (parameter is string direction)
            {
                if (direction == "+")
                {
                    Approaches++;
                }
                else if (direction == "-" && Approaches > 1)
                {
                    Approaches--;
                }
            }
        }

        private void OnChangeWorkTime(object parameter)
        {
            if (parameter is string direction)
            {
                if (direction == "+")
                {
                    _settings.WorkTimeSeconds += 5;
                }
                else if (direction == "-" && _settings.WorkTimeSeconds > 5)
                {
                    _settings.WorkTimeSeconds -= 5;
                }
                OnPropertyChanged(nameof(WorkTimeDisplay));
            }
        }

        private void OnChangeRestTime(object parameter)
        {
            if (parameter is string direction)
            {
                if (direction == "+")
                {
                    _settings.RestTimeSeconds += 5;
                }
                else if (direction == "-" && _settings.RestTimeSeconds > 5)
                {
                    _settings.RestTimeSeconds -= 5;
                }
                OnPropertyChanged(nameof(RestTimeDisplay));
            }
        }
    }
}
