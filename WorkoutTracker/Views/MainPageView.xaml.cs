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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is MainViewModel viewModel) 
            { 
                viewModel.RelaodSettingsFromPreferences();
            }
        }
    }
}
