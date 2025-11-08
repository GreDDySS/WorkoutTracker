using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Views;

public partial class AddExercisePageView : ContentPage
{
	public AddExercisePageView()
	{
		InitializeComponent();
		BindingContext = new AddExerciseViewModel();
	}
}