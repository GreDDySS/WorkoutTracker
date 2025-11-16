using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Views
{
    public partial class MainPageView : ContentPage
    {
        public MainPageView(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
