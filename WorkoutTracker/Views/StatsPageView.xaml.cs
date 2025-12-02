using System;
using System.Globalization;
using System.Threading.Tasks;
using WorkoutTracker.Converters;
using WorkoutTracker.Services;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Views
{
    public partial class StatsPageView : ContentPage
    {
        private readonly StatsViewModel _viewModel;

        public StatsPageView(StatsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName is nameof(StatsViewModel.CurrentMonth) or nameof(StatsViewModel.WorkoutDays))
                    UpdateCalendar();
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateCalendar();
        }

        private void UpdateCalendar()
        {
            if (CalendarGrid == null) return;

            CalendarGrid.Children.Clear();

            var days = _viewModel.GetCalendarDays();
            var currentMonth = _viewModel.CurrentMonth;
            var today = DateTime.Today;

            for (int i = 0; i < days.Count; i++)
            {
                var date = days[i];
                var cell = CreateDayCell(date, currentMonth, today);

                Grid.SetRow(cell, i / 7);
                Grid.SetColumn(cell, i % 7);
                CalendarGrid.Children.Add(cell);
            }
        }

        private Frame CreateDayCell(DateTime date, DateTime currentMonth, DateTime today)
        {
            bool isCurrentMonth = date.Month == currentMonth.Month && date.Year == currentMonth.Year;
            bool isToday = date.Date == today;
            bool isSelected = _viewModel.SelectedDate?.Date == date.Date;
            bool hasWorkout = _viewModel.HasWorkoutOnDate(date);
            int workoutCount = _viewModel.GetWorkoutCountOnDate(date);

            var frame = new Frame
            {
                BackgroundColor = isSelected ? Color.FromArgb("#00BFFF") :
                                  hasWorkout ? Color.FromArgb("#2C5F2D") :
                                  isCurrentMonth ? Color.FromArgb("#1A1A1A") : Color.FromArgb("#0D0D0D"),
                BorderColor = isToday ? Color.FromArgb("#00BFFF") : Colors.Transparent,
                CornerRadius = 8,
                Padding = 0,
                Margin = 0
            };

            var grid = new Grid { Padding = 4 };

            var dayLabel = new Label
            {
                Text = date.Day.ToString(),
                TextColor = !isCurrentMonth ? Color.FromArgb("#55555") :
                            isSelected ? Colors.White : Colors.LightGray,
                FontSize = 16,
                FontAttributes = isToday ? FontAttributes.Bold : FontAttributes.None,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            grid.Children.Add(dayLabel);

            if (hasWorkout && workoutCount > 0)
            {
                var badge = new Frame
                {
                    BackgroundColor = isSelected ? Color.FromArgb("#0095CC") : Color.FromArgb("#1F4620"),
                    CornerRadius = 10,
                    Padding = new Thickness(5, 2),
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, 0, 0, 0)
                };

                badge.Content = new Label
                {
                    Text = workoutCount > 9 ? "9+" : workoutCount.ToString(),
                    TextColor = isSelected ? Colors.White : Color.FromArgb("#90EE90"),
                    FontSize = 10,
                    FontAttributes = FontAttributes.Bold
                };

                grid.Children.Add(badge);
            }

            frame.Content = grid;

            if (isCurrentMonth)
            {
                frame.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => _viewModel.SelectedDate = date)
                });
            }

            return frame;
        }
    }
}