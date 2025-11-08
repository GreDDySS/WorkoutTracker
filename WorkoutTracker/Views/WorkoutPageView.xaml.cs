using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Views;

public partial class WorkoutPageView : ContentPage
{
	public WorkoutPageView()
	{
		InitializeComponent();
		BindingContext = new WorkoutViewModel();
	}
}