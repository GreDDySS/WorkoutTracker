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
		if (BindingContext is WorkoutViewModel viewModel)
		{
			_viewModel?.LoadData();
		}
    }

    private void OnEditExerciseClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int exerciseId)
        {
            _viewModel?.EditExerciseCommand?.Execute(exerciseId);
        }
    }

    private void OnEditProgramClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int programId)
        {
            _viewModel?.EditProgramCommand?.Execute(programId);
        }
    }

    private void OnStartProgramClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int programId)
        {
            _viewModel?.StartProgramCommand?.Execute(programId);
        }
    }

    private void OnDeleteExerciseClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int exerciseId)
        {
            _viewModel?.DeleteExerciseCommand?.Execute(exerciseId);
        }
    }

    private void OnDeleteProgramClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int programId)
        {
            _viewModel?.DeleteProgramCommand?.Execute(programId);
        }
    }
}