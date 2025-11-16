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
                // Используем Application.Current для получения сервисов, если Handler еще не готов
                var mauiContext = Handler?.MauiContext ?? Application.Current?.Handler?.MauiContext;
                if (mauiContext == null)
                {
                    // Если контекст еще не готов, откладываем инициализацию
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Task.Delay(100);
                        InitializeViewModel();
                    });
                    return;
                }

                InitializeViewModel();
            }
            
            // Обновляем календарь при появлении страницы (данные могут загрузиться асинхронно)
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100); // Небольшая задержка для загрузки данных
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

            // Подписываемся на изменения месяца для обновления календаря
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
            CalendarGrid.RowDefinitions.Clear();

            var days = _viewModel.GetCalendarDays();
            var currentMonth = _viewModel.CurrentMonth;

            // Создаем строки для календаря (6 недель)
            for (int row = 0; row < 6; row++)
            {
                CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            }

            // Создаем ячейки календаря
            for (int i = 0; i < days.Count; i++)
            {
                var date = days[i];
                var row = i / 7;
                var col = i % 7;
                var isCurrentMonth = date.Month == currentMonth.Month && date.Year == currentMonth.Year;
                var hasWorkout = _viewModel.HasWorkoutOnDate(date);
                var isToday = date.Date == DateTime.Now.Date;

                Color backgroundColor;
                if (isCurrentMonth)
                {
                    backgroundColor = isToday ? Color.Parse("#00FF00") : Color.Parse("#2C2C2C");
                }
                else
                {
                    backgroundColor = Color.Parse("#1A1A1A");
                }

                var frame = new Frame
                {
                    BackgroundColor = backgroundColor,
                    CornerRadius = 8,
                    Padding = 5,
                    HasShadow = false
                };

                var stackLayout = new VerticalStackLayout
                {
                    Spacing = 2,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                var dayLabel = new Label
                {
                    Text = date.Day.ToString(),
                    TextColor = isCurrentMonth ? Colors.White : Colors.Gray,
                    FontSize = 14,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start
                };

                stackLayout.Children.Add(dayLabel);

                if (hasWorkout)
                {
                    var workoutCount = _viewModel.GetWorkoutCountOnDate(date);
                    var emojiLabel = new Label
                    {
                        Text = "💪",
                        FontSize = 12,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.End
                    };
                    stackLayout.Children.Add(emojiLabel);

                    if (workoutCount > 1)
                    {
                        var countLabel = new Label
                        {
                            Text = workoutCount.ToString(),
                            TextColor = Colors.Yellow,
                            FontSize = 10,
                            HorizontalOptions = LayoutOptions.Center
                        };
                        stackLayout.Children.Add(countLabel);
                    }
                }

                frame.Content = stackLayout;

                // Добавляем обработчик нажатия
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) =>
                {
                    if (isCurrentMonth)
                    {
                        _viewModel.OnDateSelected(date);
                    }
                };
                frame.GestureRecognizers.Add(tapGesture);

                CalendarGrid.Add(frame, col, row);
            }
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

