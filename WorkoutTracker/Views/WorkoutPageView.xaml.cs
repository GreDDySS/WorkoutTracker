using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Views;

public partial class WorkoutPageView : ContentPage
{
	private WorkoutViewModel _viewModel;

	public WorkoutPageView()
	{
		InitializeComponent();
		_viewModel = new WorkoutViewModel();
		BindingContext = _viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		_viewModel?.LoadData();
    }
}