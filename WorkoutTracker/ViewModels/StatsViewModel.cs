using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using WorkoutTracker.Base;
using WorkoutTracker.Models;
using WorkoutTracker.Services;

namespace WorkoutTracker.ViewModels
{
    public class StatsViewModel : BaseViewModel
    {
        private readonly IWorkoutHistoryService _historyService;

        private DateTime _currentMonth;
        private DateTime? _selectedDate;
        private ObservableCollection<WorkoutHistory> _workoutsForSelectedDate = new();
        private Dictionary<DateTime, int> _workoutDays = new();

        public StatsViewModel(IWorkoutHistoryService historyService)
        {
            _historyService = historyService;
            _currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            PreviousMonthCommand = new RelayCommand(_ => OnPreviousMonth());
            NextMonthCommand = new RelayCommand(_ => OnNextMonth());
            DateSelectedCommand = new RelayCommand(o => { if (o is DateTime d) SelectedDate = d; });

            _ = LoadMonthDataAsync();
        }

        public string CurrentMonthName => _currentMonth.ToString("MMMM yyyy", new CultureInfo("ru-RU"));

        public DateTime CurrentMonth
        {
            get => _currentMonth;
            private set => SetProperty(ref _currentMonth, value);
        }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    if (value.HasValue)
                       _ = LoadWorkoutsForDateAsync(value.Value);
                    else
                        WorkoutsForSelectedDate.Clear();
                }
            }
        }

        public ObservableCollection<WorkoutHistory> WorkoutsForSelectedDate
        {
            get => _workoutsForSelectedDate;
            private set => SetProperty(ref _workoutsForSelectedDate, value);
        }
        
        public Dictionary<DateTime, int> WorkoutDays => _workoutDays;

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand DateSelectedCommand { get; }

        public bool HasWorkoutOnDate(DateTime date) => _workoutDays.ContainsKey(date.Date);

        public int GetWorkoutCountOnDate(DateTime date) =>
            _workoutDays.TryGetValue(date.Date, out var count) ? count : 0;

        public List<DateTime> GetCalendarDays()
        {
            var days = new List<DateTime>(42);

            var firstDay = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            var start = firstDay.AddDays(-(int)firstDay.DayOfWeek + 1);

            for (int i = 0; i < 42; i++)
            {
                days.Add(start.AddDays(i));
            }

            return days;
        }

        private async Task LoadMonthDataAsync()
        {
            _workoutDays = await _historyService.GetWorkoutDaysInMonthAsync(_currentMonth.Year, _currentMonth.Month);
            
            OnPropertyChanged(nameof(CurrentMonthName));
            OnPropertyChanged(nameof(CurrentMonth));
            OnPropertyChanged(nameof(WorkoutDays));
            OnPropertyChanged(string.Empty);
        }

        private async Task LoadWorkoutsForDateAsync(DateTime date)
        {
            try
            {
                var workouts = await _historyService.GetWorkoutsByDateAsync(date);
                WorkoutsForSelectedDate.Clear();
                foreach (var workout in workouts)
                {
                    WorkoutsForSelectedDate.Add(workout);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке тренировок: {ex.Message}");
            }
        }

        private async void OnPreviousMonth()
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            SelectedDate = null;
            await LoadMonthDataAsync();
        }

        private async void OnNextMonth()
        {
            _currentMonth = _currentMonth.AddMonths(1);
            SelectedDate = null;
            await LoadMonthDataAsync();
        }

        public void OnDateSelected(DateTime date)
        {
            SelectedDate = date;
        }
    }
}

