using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Views;

public partial class AddProgramPageView : ContentPage
{
    public AddProgramPageView()
    {
        InitializeComponent();
        BindingContext = new AddProgramViewModel();
    }
}