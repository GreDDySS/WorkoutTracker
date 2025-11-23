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
        private StatsViewModel _viewModel;

        public StatsPageView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
            {
                var mauiContext = Handler?.MauiContext ?? Application.Current?.Handler?.MauiContext;
                if (mauiContext == null)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Task.Delay(100);
                        InitializeViewModel();
                    });
                    return;
                }

                InitializeViewModel();
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);
                UpdateCalendar();
            });
        }

        private void InitializeViewModel()
        {
            if (_viewModel != null) return;

            var mauiContext = Handler?.MauiContext ?? Application.Current?.Handler?.MauiContext;
            if (mauiContext == null) return;

            var historyService = mauiContext.Services.GetService<IWorkoutHistoryService>();
            if (historyService == null) return;

            _viewModel = new StatsViewModel(historyService);
            BindingContext = _viewModel;

            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StatsViewModel.CurrentMonth) ||
                e.PropertyName == nameof(StatsViewModel.WorkoutDays))
            {
                UpdateCalendar();
            }
        }

        private void UpdateCalendar()
        {
            if (_viewModel == null || CalendarGrid == null) return;

            CalendarGrid.Children.Clear();

            var days = _viewModel.GetCalendarDays();
            var currentMonth = _viewModel.CurrentMonth;
            var today = DateTime.Today;

            for (int i = 0; i < days.Count; i++)
            {
                var date = days[i];
                var row = i / 7;
                var col = i % 7;

                var dayCell = CreateDayCell(date, currentMonth, today);

                Grid.SetRow(dayCell, row);
                Grid.SetColumn(dayCell, col);

                CalendarGrid.Children.Add(dayCell);
            }
        }

        private Frame CreateDayCell(DateTime date, DateTime currentMonth, DateTime today)
        {
            bool isCurrentMonth = date.Month == currentMonth.Month && date.Year == currentMonth.Year;
            bool isToday = date.Date == today;
            bool isSelected = _viewModel.SelectedDate?.Date == date.Date;
            bool hasWorkout = _viewModel.HasWorkoutOnDate(date);
            int workoutCount = _viewModel.GetWorkoutCountOnDate(date);

            // Определяем цвет фона
            Color backgroundColor;
            if (isSelected)
            {
                backgroundColor = Color.FromArgb("#00BFFF");
            }
            else if (hasWorkout)
            {
                backgroundColor = Color.FromArgb("#2C5F2D");
            }
            else if (!isCurrentMonth)
            {
                backgroundColor = Color.FromArgb("#0D0D0D");
            }
            else
            {
                backgroundColor = Color.FromArgb("#1A1A1A");
            }

            // Определяем цвет границы для сегодняшнего дня
            Color borderColor = isToday ? Color.FromArgb("#00BFFF") : Colors.Transparent;

            // Цвет текста
            Color textColor = !isCurrentMonth ? Color.FromArgb("#555555") :
                             isSelected ? Colors.White :
                             Colors.LightGray;

            var frame = new Frame
            {
                BackgroundColor = backgroundColor,
                BorderColor = borderColor,
                HasShadow = false,
                CornerRadius = 8,
                Padding = 0,
                Margin = 0,
                HeightRequest = 50,
                WidthRequest = 50,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            var grid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Star }
                },
                Padding = 4,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            // Основная цифра дня
            var dayLabel = new Label
            {
                Text = date.Day.ToString(),
                TextColor = textColor,
                FontSize = 16,
                FontAttributes = isToday ? FontAttributes.Bold : FontAttributes.None,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            grid.Children.Add(dayLabel);

            // Добавляем индикатор количества тренировок
            if (hasWorkout && workoutCount > 0)
            {
                var indicatorGrid = new Grid
                {
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, 2, 2, 0)
                };

                var countBadge = new Frame
                {
                    BackgroundColor = isSelected ? Color.FromArgb("#0095CC") : Color.FromArgb("#1F4620"),
                    CornerRadius = 8,
                    Padding = new Thickness(4, 1),
                    HasShadow = false,
                    HeightRequest = 16,
                    MinimumWidthRequest = 16
                };

                var countLabel = new Label
                {
                    Text = workoutCount > 9 ? "9+" : workoutCount.ToString(),
                    TextColor = isSelected ? Colors.White : Color.FromArgb("#90EE90"),
                    FontSize = 9,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                countBadge.Content = countLabel;
                indicatorGrid.Children.Add(countBadge);
                grid.Children.Add(indicatorGrid);
            }

            frame.Content = grid;

            // Добавляем обработчик нажатия только для дней текущего месяца
            if (isCurrentMonth)
            {
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => _viewModel.OnDateSelected(date);
                frame.GestureRecognizers.Add(tapGesture);
            }

            return frame;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }
    }
}