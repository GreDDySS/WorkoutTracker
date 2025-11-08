using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Views;

public partial class CreateWorkoutPageView : ContentPage
{
	public CreateWorkoutPageView()
	{
		InitializeComponent();
		BindingContext = new CreateWorkoutViewModel();
	}
}