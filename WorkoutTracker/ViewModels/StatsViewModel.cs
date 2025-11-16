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
        private ObservableCollection<WorkoutHistory> _workoutsForSelectedDate;
        private Dictionary<DateTime, int> _workoutDays;

        public StatsViewModel(IWorkoutHistoryService historyService)
        {
            _historyService = historyService;
            _currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            _workoutsForSelectedDate = new ObservableCollection<WorkoutHistory>();
            _workoutDays = new Dictionary<DateTime, int>();

            PreviousMonthCommand = new RelayCommand(OnPreviousMonth);
            NextMonthCommand = new RelayCommand(OnNextMonth);
            DateSelectedCommand = new RelayCommand(OnDateSelectedCommand);

            LoadMonthData();
        }

        public string CurrentMonthName => _currentMonth.ToString("MMMM yyyy", new CultureInfo("ru-RU"));
        public DateTime CurrentMonth => _currentMonth;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    if (value.HasValue)
                    {
                        LoadWorkoutsForDate(value.Value);
                    }
                    else
                    {
                        WorkoutsForSelectedDate.Clear();
                    }
                }
            }
        }

        public ObservableCollection<WorkoutHistory> WorkoutsForSelectedDate
        {
            get => _workoutsForSelectedDate;
            set => SetProperty(ref _workoutsForSelectedDate, value);
        }

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand DateSelectedCommand { get; }

        public bool HasWorkoutOnDate(DateTime date)
        {
            return _workoutDays.ContainsKey(date.Date);
        }

        public int GetWorkoutCountOnDate(DateTime date)
        {
            return _workoutDays.TryGetValue(date.Date, out var count) ? count : 0;
        }

        public List<DateTime> GetCalendarDays()
        {
            var days = new List<DateTime>();
            var firstDayOfMonth = _currentMonth;
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            
            // Начинаем с понедельника недели, в которой находится первое число месяца
            var startDate = firstDayOfMonth;
            var dayOfWeek = (int)startDate.DayOfWeek;
            // В C# DayOfWeek: Sunday = 0, Monday = 1, ..., Saturday = 6
            // Нужно преобразовать в: Monday = 0, Tuesday = 1, ..., Sunday = 6
            var adjustedDayOfWeek = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
            startDate = startDate.AddDays(-adjustedDayOfWeek);

            // Добавляем 42 дня (6 недель)
            for (int i = 0; i < 42; i++)
            {
                days.Add(startDate.AddDays(i));
            }

            return days;
        }

        private async void LoadMonthData()
        {
            try
            {
                _workoutDays = await _historyService.GetWorkoutDaysInMonthAsync(_currentMonth.Year, _currentMonth.Month);
                OnPropertyChanged(nameof(CurrentMonthName));
                OnPropertyChanged(nameof(CurrentMonth));
                // Уведомляем об изменении данных для обновления календаря
                OnPropertyChanged(nameof(WorkoutDays));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке данных месяца: {ex.Message}");
            }
        }

        // Свойство для уведомления об изменении данных
        public Dictionary<DateTime, int> WorkoutDays => _workoutDays;

        private async void LoadWorkoutsForDate(DateTime date)
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

        private void OnPreviousMonth(object parameter)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            SelectedDate = null;
            LoadMonthData();
        }

        private void OnNextMonth(object parameter)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            SelectedDate = null;
            LoadMonthData();
        }

        public void OnDateSelected(DateTime date)
        {
            SelectedDate = date;
        }

        private void OnDateSelectedCommand(object parameter)
        {
            if (parameter is DateTime date)
            {
                OnDateSelected(date);
            }
        }
    }
}

