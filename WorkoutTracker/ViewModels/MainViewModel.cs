using System.Collections.Generic;
using System.Windows.Input;
using WorkoutTracker.Base;
using WorkoutTracker.Models;

namespace WorkoutTracker.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private WorkoutSettings _settings = new WorkoutSettings();

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

        public MainViewModel()
        {
            StartWorkoutCommand = new RelayCommand(OnStartWorkout);
            ChangeApproachesCommand = new RelayCommand(OnChangeApproaches);
            ChangeWorkTimeCommand = new RelayCommand(OnChangeWorkTime);
            ChangeRestTimeCommand = new RelayCommand(OnChangeRestTime);
        }

        private async void OnStartWorkout(object parameter)
        {
            // Navigate to TimerPage with settings
            if (Shell.Current != null)
            {
                // Store settings in AppShell for navigation
                AppShell.NavigationSettings = _settings;
                await Shell.Current.GoToAsync("TimerPage");
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
