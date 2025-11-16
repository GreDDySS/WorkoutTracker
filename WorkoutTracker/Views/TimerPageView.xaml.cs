using WorkoutTracker.Models;
using WorkoutTracker.Services;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Views;

public partial class TimerPageView : ContentPage
{
    private TimerViewModel _viewModel;

    public TimerPageView()
	{
		InitializeComponent();
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        WorkoutSettings settings = AppShell.NavigationSettings ?? new WorkoutSettings();
        AppShell.NavigationSettings = null; 

        var timerService = Handler.MauiContext.Services.GetService<IWorkoutTimerService>();
        var stateService = Handler.MauiContext.Services.GetService<IWorkoutStateService>();
        var navigationService = Handler.MauiContext.Services.GetService<INavigationService>();
        var historyService = Handler.MauiContext.Services.GetService<IWorkoutHistoryService>();

        _viewModel = new TimerViewModel(timerService, stateService, navigationService, historyService, settings);
        BindingContext = _viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel?.Dispose();
    }
}