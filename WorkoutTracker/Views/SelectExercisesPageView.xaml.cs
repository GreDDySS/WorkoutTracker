using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Views;

public partial class SelectExercisesPageView : ContentPage
{
	public SelectExercisesPageView()
	{
		InitializeComponent();
		BindingContext = new SelectExercisesViewModel();
	}
}